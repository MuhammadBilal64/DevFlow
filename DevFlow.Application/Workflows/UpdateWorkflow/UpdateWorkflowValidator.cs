using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Workflows.CreateWorkflow.Validators;
using FluentValidation;

namespace DevFlow.Application.Workflows.UpdateWorkflow
{
    public class UpdateWorkflowValidator:AbstractValidator<UpdateWorkflowCommand>
    {
        public UpdateWorkflowValidator() {

            RuleFor(x => x.WorkflowId)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleForEach(x => x.Actions)
                .SetValidator(new WorkflowActionDtoValidator());

            RuleForEach(x => x.Conditions)
                .SetValidator(new WorkflowConditionDtoValidator());

            RuleFor(x => x.Actions)
    .Must(actions =>
        actions.Select(a => a.Order).Distinct().Count() == actions.Count)
    .WithMessage("Workflow action order values must be unique.");
        }

    }
}
