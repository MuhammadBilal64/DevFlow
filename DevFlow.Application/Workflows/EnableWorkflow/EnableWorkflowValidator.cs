using DevFlow.Application.Workflows.EnableWorkflow;
using FluentValidation;

public class EnableWorkflowValidator
    : AbstractValidator<EnableWorkflowCommand>
{
    public EnableWorkflowValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0);

        RuleFor(x => x.WorkflowId)
            .GreaterThan(0);
    }
}