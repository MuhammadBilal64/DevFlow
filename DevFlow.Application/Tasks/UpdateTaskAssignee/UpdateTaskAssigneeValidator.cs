using FluentValidation;

namespace DevFlow.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeValidator : AbstractValidator<UpdateTaskAssigneeCommand>
    {
        public UpdateTaskAssigneeValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0);

            RuleFor(x => x.NewAssigneeId)
     .GreaterThan(0)
     .When(x => x.NewAssigneeId.HasValue);
            RuleFor(x => x.ProjectId)
    .GreaterThan(0);
        }
    }
}
