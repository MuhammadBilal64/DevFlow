using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdValidator:AbstractValidator<GetTaskByIdQuery>
    {
        public GetTaskByIdValidator()
        {
            RuleFor(x => x.TaskId).GreaterThan(0);
        }
    }
}
