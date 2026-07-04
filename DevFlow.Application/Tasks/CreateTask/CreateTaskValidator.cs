using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Tasks.CreateTask
{
    public class CreateTaskValidator:AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator() {
            RuleFor(t => t.Description).MaximumLength(200);
            RuleFor(t => t.Title).NotEmpty().MaximumLength(50);
            RuleFor(i => i.Priority).IsInEnum();
            RuleFor(x => x.ProjectId)
                    .GreaterThan(0);
          
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.DueDate.HasValue);


        }
    }
}
