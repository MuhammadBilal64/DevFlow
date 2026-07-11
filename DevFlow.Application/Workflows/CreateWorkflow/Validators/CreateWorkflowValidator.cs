using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workflows.CreateWorkflow.Validators
{
    public class CreateWorkflowValidator:AbstractValidator<CreateWorkflowCommand>
    {
        public CreateWorkflowValidator() {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Description).MaximumLength(500);
            RuleFor(i => i.Trigger).IsInEnum();
            RuleFor(x => x.Actions)
                .NotEmpty()
                .WithMessage("Workflow must contain at least one action."); 
            RuleFor(x => x.Conditions)
                .NotNull();
            RuleForEach(x => x.Conditions)
     .SetValidator(new WorkflowConditionDtoValidator());
            RuleForEach(x => x.Actions).SetValidator(new WorkflowActionDtoValidator());



        }
    }
}
