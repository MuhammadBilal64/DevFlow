using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using FluentValidation;

namespace DevFlow.Application.Common.Validators
{
    public class PaginationRequestValidator:AbstractValidator<PaginationRequest>
    {
     public PaginationRequestValidator() {
            RuleFor(x => x.PageNumber)
        .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

        }
    
    }
}
