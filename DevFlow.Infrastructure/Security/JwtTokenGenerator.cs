using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevFlow.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSetting _settings;
        public JwtTokenGenerator(IOptions<JwtSetting>settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
           {
               new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
               new Claim(ClaimTypes.Email,user.Email),
               new Claim(ClaimTypes.Role,user.Role.ToString())
           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var cred= new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
