using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowValidator
        : AbstractValidator<EnableWorkflowCommand>
    {
        public EnableWorkflowValidator()
        {
            RuleFor(x => x.WorkflowId)
                .GreaterThan(0);
        }
    }

}
