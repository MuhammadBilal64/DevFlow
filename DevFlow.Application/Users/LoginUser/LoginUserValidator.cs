using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserValidator:AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email Is Required")
            .EmailAddress()
            .WithMessage("Invalid Email Format");

            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("Password is Required");
                
        }

    }
}
