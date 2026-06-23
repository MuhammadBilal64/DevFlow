using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Projects.CreateProject
{
    public class CreateProjectValidator:AbstractValidator<CreateProjectCommand>
    {
       public CreateProjectValidator() {
            RuleFor(x => x.ProjectName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
            
        }
    }
}
