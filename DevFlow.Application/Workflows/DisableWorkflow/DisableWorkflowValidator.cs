using DevFlow.Application.Workflows.DisableWorkflow;
using FluentValidation;

public class DisableWorkflowValidator : AbstractValidator<DisableWorkflowCommand>
{
    public DisableWorkflowValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0);

        RuleFor(x => x.WorkflowId)
            .GreaterThan(0);
    }
}