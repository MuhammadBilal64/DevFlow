using DevFlow.Application.Workflows.GetWorkflowById;
using FluentValidation;

public class GetWorkflowByIdValidator
    : AbstractValidator<GetWorkflowByIdQuery>
{
    public GetWorkflowByIdValidator()
    {
        RuleFor(x => x.WorkflowId)
            .GreaterThan(0);
    }
}