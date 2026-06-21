using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workspaces.CreateWorkspace
{
    public class CreateWorkspaceValidator:AbstractValidator<CreateWorkspaceCommand>
    {
        public CreateWorkspaceValidator() {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        
        }
    }
}
