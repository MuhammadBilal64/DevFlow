using FluentValidation;

namespace DevFlow.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0);

            RuleFor(x => x.TaskStatus)
                .IsInEnum();
            RuleFor(x => x.ProjectId)
      .GreaterThan(0);
        }
    }
}
