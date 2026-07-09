using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workflows.CreateWorkflow.Validators
{
    public class CreateWorkflowConditionDtoValidator :  AbstractValidator<CreateWorkflowConditionDto>

    {
        public CreateWorkflowConditionDtoValidator()
        {
            RuleFor(x => x.Field)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Operator)
                .IsInEnum();

            RuleFor(x => x.Value)
                .NotEmpty()
                .MaximumLength(300);
        }

    }
}
