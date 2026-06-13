using System.Text;
using DevFlow.Application.Abstractions;
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
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
