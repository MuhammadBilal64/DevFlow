using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task<RefreshToken?>GetByTokenAsync(string token);



    }
}
