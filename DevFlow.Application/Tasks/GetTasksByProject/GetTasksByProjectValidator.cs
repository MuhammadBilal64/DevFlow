using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectValidator
     : AbstractValidator<GetTasksByProjectQuery>
    {
        public GetTasksByProjectValidator()
        {
            Include(new PaginationRequestValidator());
        }
    }
}
