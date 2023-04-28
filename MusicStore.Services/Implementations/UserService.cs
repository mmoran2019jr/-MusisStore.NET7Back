﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MusicStore.Services.Implementations;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly UserManager<MusicStoreUserIdentity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;

    public UserService(ILogger<UserService> logger,
        IOptions<AppSettings> options,
        UserManager<MusicStoreUserIdentity> userManager,
        RoleManager<IdentityRole> roleManager,
        ICustomerRepository customerRepository,
        IEmailService emailService)
    {
        _logger = logger;
        _options = options;
        _userManager = userManager;
        _roleManager = roleManager;
        _customerRepository = customerRepository;
        _emailService = emailService;
    }

    public async Task<LoginDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        var response = new LoginDtoResponse();

        try
        {
            var identity = await _userManager.FindByEmailAsync(request.UserName);

            if (identity == null)
            {
                throw new ApplicationException("Usuario no existe");
            }

            var result = await _userManager.CheckPasswordAsync(identity, request.Password);
            if (!result)
            {
                response.Success = false;
                response.ErrorMessage = "Usuario o clave incorrectos";

                _logger.LogWarning($"Error de autenticacion para el usuario {request.UserName}");

                return response;
            }

            var roles = await _userManager.GetRolesAsync(identity);

            var expiredDate = DateTime.Now.AddDays(1);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.Email, request.UserName),
                new Claim(ClaimTypes.Expiration, expiredDate.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            response.Roles = new List<string>();
            response.Roles.AddRange(roles);

            // Creacion de Token JWT
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Jwt.SecretKey));

            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(_options.Value.Jwt.Issuer,
                _options.Value.Jwt.Audience,
                claims,
                DateTime.Now,
                expiredDate);

            var token = new JwtSecurityToken(header, payload);

            response.Token = new JwtSecurityTokenHandler().WriteToken(token);
            response.FullName = $"{identity.FirstName} {identity.LastName}";
            response.Success = true;
            response.email = identity.Email;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponseGeneric<string>> RegisterAsync(RegisterDtoRequest request)
    {
        var response = new BaseResponseGeneric<string>();

        try
        {
            var user = new MusicStoreUserIdentity
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var userIdentity = await _userManager.FindByNameAsync(user.UserName);

                if (userIdentity != null)
                {
                    if (!await _roleManager.RoleExistsAsync(request.Role))
                        await _roleManager.CreateAsync(new IdentityRole(request.Role));

                    await _userManager.AddToRoleAsync(userIdentity, request.Role);

                    // Creamos el objeto Customer con el registro del usuario.
                    var customer = new Customer
                    {
                        FullName = $"{request.FirstName} {request.LastName}",
                        Email = request.Email,
                    };

                    await _customerRepository.AddAsync(customer);

                    await _emailService.SendEmailAsync(request.Email, $"Creacion de Cuenta para {customer.FullName}",
                        $"Se ha creado su cuenta <strong>{user.UserName}</strong> exitosamente en nuestro sistema, bienvenido!");

                    response.Success = true;
                    response.Data = user.Id;
                }
            }
            else
            {
                response.Success = false;
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterAsync");
            response.Success = false;
            response.ErrorMessage = "Error in RegisterAsync";
        }

        return response;
    }

    public async Task<BaseResponse> RequestTokenToResetPasswordAsync(DtoRequestPassword request)
    {
        var response = new BaseResponse();

        try
        {
            //var ajusteCadena = request.Email.Replace($"\"{request.Email}\"", $"{request.Email}");

            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("Usuario no existe");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(userIdentity);

            // Enviar un correo
            await _emailService.SendEmailAsync(request.Email, "Restablecer contraseña",
                $"Hola {userIdentity.FirstName} {userIdentity.LastName} para restablecer su contraseña copie el siguiente token: {token}");

            _logger.LogInformation("Se envio correo con solicitud para reseteo a {email}", request.Email);

            response.Success = true;
            response.Token = token;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "No se pudo completar la solicitud";
            _logger.LogError(ex, "Error al resetear password {Message}", ex.Message);
        }

        return response;
    }

    public async Task<BaseResponse> ResetPasswordAsync(DtoResetPassword request)
    {
        var response = new BaseResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("Usuario no existe");
            }

            var result = await _userManager.ResetPasswordAsync(userIdentity, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }
            else
            {
                await _emailService.SendEmailAsync(userIdentity.Email!, "Reseteo de password",
                    $"Hola {userIdentity.FirstName} se ha reseteado tu password de forma exitosa");

                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "No se pudo completar la solicitud";
            _logger.LogError(ex, "Error al resetear password {Message}", ex.Message);
        }

        return response;
    }

    public async Task<BaseResponse> ChangePasswordAsync(DtoChangePassword request)
    {
        var response = new BaseResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("Usuario no existe");
            }

            var result = await _userManager.ChangePasswordAsync(userIdentity, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }
            else
            {
                await _emailService.SendEmailAsync(userIdentity.Email!, "Cambio de password",
                    $"Hola {userIdentity.FirstName}, se ha cambiado el password de tu usuario de acuerdo a lo solicitado");
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "No se pudo completar la solicitud";
            _logger.LogError(ex, "Error al cambiar password {Message}", ex.Message);
        }

        return response;
    }
}