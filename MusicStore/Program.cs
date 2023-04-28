using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Implementations;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Profiles;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var corsConfig = "MusicStoreAPI";

var logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information)
    //.WriteTo.File("..\\log.txt",
    //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
    //    rollingInterval: RollingInterval.Day,
    //    restrictedToMinimumLevel:LogEventLevel.Warning)
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("Database"),
        new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true,
            TableName = "ApiLogs"
        }, restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();

builder.Logging.AddSerilog(logger);

builder.Services.AddCors(setup =>
{
    setup.AddPolicy(corsConfig, x =>
    {
        x.WithOrigins(builder.Configuration.GetValue<string>("Origins")!);
        
        x.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<AppSettings>(builder.Configuration);

// Add services to the container.
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<MusicStoreUserIdentity, IdentityRole>(policies =>
{
    policies.Password.RequireDigit = false;
    policies.Password.RequireLowercase = false;
    policies.Password.RequireUppercase = false;
    policies.Password.RequireNonAlphanumeric = false;
    policies.Password.RequiredLength = 5;

    policies.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<MusicStoreDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GenreProfile>();
    config.AddProfile<ConcertProfile>();
    config.AddProfile<SaleProfile>();
});

builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IGenreService, GenreService>();

builder.Services.AddTransient<IConcertRepository, ConcertRepository>();
builder.Services.AddTransient<IConcertService, ConcertService>();

builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<ISaleRepository, SaleRepository>();
builder.Services.AddTransient<ISaleService, SaleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailService>();


if (builder.Environment.IsDevelopment())
    builder.Services.AddTransient<IFileUploader, FileUploader>();
else
    builder.Services.AddTransient<IFileUploader, AzureBlobStorageUploader>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admins", policy =>
    {
        policy.RequireRole("Administrador");
        //policy.RequireClaim(ClaimTypes.Gender);
    });

});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = "Bearer";
    x.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(x =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException());

    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(corsConfig);

app.MapControllers();

app.MapGet("api/Genres", async (IGenreService service) => await service.ListAsync());


// GET api/Genres/5
app.MapGet("api/Genres/{id:int}", async (IGenreService service, int id) =>
{
    var response = await service.FindByIdAsync(id);

    return response.Success ? Results.Ok(response) : Results.NotFound(response);

});

app.MapPost("api/Genres", async (IGenreService service, GenreDtoRequest request) =>
{
    var response = await service.AddAsync(request);

    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
}).RequireAuthorization("Admins");

app.MapPut("api/Genres/{id:int}", async (IGenreService service, int id, GenreDtoRequest request) =>
{
    var response = await service.UpdateAsync(id, request);

    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
}).RequireAuthorization("Admins");

app.MapDelete("api/Genres/{id:int}", async (IGenreService service, int id) =>
{
    var response = await service.DeleteAsync(id);

    return response.Success ? Results.Ok(response) : Results.NotFound(response);
}).RequireAuthorization("Admins");

app.Run();



