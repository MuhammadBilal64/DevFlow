using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Users.RefreshToken
{
    public class RefreshTokenValidator:AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator() {

            RuleFor(x => x.RefreshToken).NotNull();
        }
    }
}
