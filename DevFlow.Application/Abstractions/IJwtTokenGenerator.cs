using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IJwtTokenGenerator
    {
    string GenerateToken(User user);
    }
}
