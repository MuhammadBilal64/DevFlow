using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class RefreshTokenRepository:IRefreshTokenRepository
    {
        private readonly DevFlowDbContext _context;
        public RefreshTokenRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(u=>u.Token==token);

        }

        public async Task UpdateAsync(RefreshToken Token)
        {
             _context.RefreshTokens.Update(Token);
            await _context.SaveChangesAsync();


        }
    }
}
