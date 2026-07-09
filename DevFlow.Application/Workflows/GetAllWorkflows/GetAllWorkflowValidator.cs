using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowValidator:AbstractValidator<GetAllWorkflowsQuery>
    {
        public GetAllWorkflowValidator()
        {
            RuleFor(x => x.Trigger)
           .IsInEnum()
           .When(x => x.Trigger.HasValue);
            Include(new PaginationRequestValidator());
        }
    }
}
