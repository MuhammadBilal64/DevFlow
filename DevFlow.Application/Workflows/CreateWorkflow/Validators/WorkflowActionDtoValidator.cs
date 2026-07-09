using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workflows.WorkflowDtos;
using FluentValidation;

namespace DevFlow.Application.Workflows.CreateWorkflow.Validators
{
    public class WorkflowActionDtoValidator
    
         : AbstractValidator<WorkflowActionDto>
    {
    
        public WorkflowActionDtoValidator()
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
