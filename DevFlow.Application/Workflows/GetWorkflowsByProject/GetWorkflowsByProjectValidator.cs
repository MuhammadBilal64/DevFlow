using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetWorkflowsByProjectValidator:AbstractValidator<GetWorkflowsByProjectQuery>
    {
        public GetWorkflowsByProjectValidator()
        {
            RuleFor(x => x.Trigger)
           .IsInEnum()
           .When(x => x.Trigger.HasValue);
            Include(new PaginationRequestValidator());
        }
    }
}
