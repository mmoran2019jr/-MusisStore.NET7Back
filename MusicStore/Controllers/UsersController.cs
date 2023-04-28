using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginDtoResponse), 200)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDtoRequest request)
        {
            var response = await _service.LoginAsync(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(BaseResponseGeneric<string>), 200)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDtoRequest request)
        {
            var response = await _service.RegisterAsync(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("SendTokenToResetPassword")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        public async Task<IActionResult> SendTokenToResetPassword([FromBody] DtoRequestPassword request)
        {
            return Ok(await _service.RequestTokenToResetPasswordAsync(request));
        }

        [HttpPost("ResetPassword")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        public async Task<IActionResult> ResetPassword([FromBody] DtoResetPassword request)
        {
            return Ok(await _service.ResetPasswordAsync(request));
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] DtoChangePassword request)
        {
            return Ok(await _service.ChangePasswordAsync(request));
        }
    }
}
