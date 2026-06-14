using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IUserRepository
    {
        Task <bool>ExistByEmailAsync(string email);
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string Email);
        Task<User?>GetByUserIdAsync(int userId);
    }
}
