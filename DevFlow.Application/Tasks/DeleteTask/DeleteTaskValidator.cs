using FluentValidation;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskValidator()
        {
            RuleFor(x => x.ProjectId)
        .GreaterThan(0);

            RuleFor(x => x.TaskId)
                .GreaterThan(0);
        }
    }
}
