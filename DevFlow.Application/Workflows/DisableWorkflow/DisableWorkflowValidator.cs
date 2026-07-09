using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowValidator:AbstractValidator<DisableWorkflowCommand>
    {
        public DisableWorkflowValidator()
        {
            RuleFor(x => x.WorkflowId)
                .GreaterThan(0);
        }
    }
}
