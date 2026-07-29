using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Projects.GetProjectById
{
    public class GetProjectByIdValidator:AbstractValidator<GetProjectByIdQuery>
    {
        public GetProjectByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
