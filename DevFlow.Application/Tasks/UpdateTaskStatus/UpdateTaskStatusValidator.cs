using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}
