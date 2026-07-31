using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetWorkflowsByProjectValidator : AbstractValidator<GetWorkflowsByProjectQuery>
    {
        public GetWorkflowsByProjectValidator()
        {
            RuleFor(x => x.Trigger)
           .IsInEnum()
           .When(x => x.Trigger.HasValue);
            RuleFor(x => x.ProjectId)
    .GreaterThan(0);
            Include(new PaginationRequestValidator());
        }
    }
}
