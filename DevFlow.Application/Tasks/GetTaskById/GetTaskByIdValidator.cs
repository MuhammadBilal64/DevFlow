using FluentValidation;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdValidator : AbstractValidator<GetTaskByIdQuery>
    {
        public GetTaskByIdValidator()
        {
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.TaskId).GreaterThan(0);
        }
    }
}
