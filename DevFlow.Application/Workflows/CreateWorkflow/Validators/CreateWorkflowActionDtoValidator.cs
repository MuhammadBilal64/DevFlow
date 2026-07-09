using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workflows.CreateWorkflow.Validators
{
    public class CreateWorkflowActionDtoValidator
    
         : AbstractValidator<CreateWorkflowActionDto>
    {
    
        public CreateWorkflowActionDtoValidator()
        {
            RuleFor(x => x.ActionType)
                .IsInEnum();

            RuleFor(x => x.Parameters)
                .NotEmpty();

            RuleFor(x => x.Order)
                .GreaterThan(0);
        }
    }
}
