using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Users.RegisterUser
{
    public interface IUserRepository
    {
        Task <bool>ExistByEmailAsync(string email);
        Task AddAsync(User user);
    }
}
