using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0);
        }
    }
}
