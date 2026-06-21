using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Users.RegisterUser
{
    public class RegisterUserHandler:IRequestHandler<RegisterUserCommand,RegisterUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterUserHandler(IUnitOfWork unitOfWork,IUserRepository userRepository,IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork= unitOfWork;
        }
      
        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _userRepository.ExistByEmailAsync(request.Email);
            if (exists)
            {
                throw new ConflictException("Email Already Exist");
               
            }
            var Password = _passwordHasher.Hash(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = UserRole.Member,
                CreatedAt = DateTime.UtcNow,
                PasswordHash=Password


            };
           await  _userRepository.AddAsync(user);
           await _unitOfWork.SaveChangesAsync();
            return new RegisterUserResult
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,

            };

        }

       
    }
}
