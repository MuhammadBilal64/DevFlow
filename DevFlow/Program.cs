using System.Text;
using DevFlow.Api.Middleware;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Users.LoginUser;
using DevFlow.Application.Users.LogoutUser;
using DevFlow.Application.Users.RefreshToken;
using DevFlow.Application.Users.RegisterUser;
using DevFlow.Infrastructure.Persistence;
using DevFlow.Infrastructure.Repositories;
using DevFlow.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
// Program.cs
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<LoginUserHandle>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<RefreshTokenHandle>();
builder.Services.AddScoped<LogoutHandle>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    var jwtsettings = builder.Configuration.GetSection("Jwt").Get<JwtSetting>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey = true,
        ValidIssuer=jwtsettings!.Issuer,
        ValidAudience=jwtsettings.Audience,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings.Key))



    };   

});


builder.Services.AddDbContext<DevFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevFlowDb")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
