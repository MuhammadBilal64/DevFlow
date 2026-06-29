using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using FluentValidation;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectValidator:AbstractValidator<PagedResult<GetTasksByProjectResult>>
    {

    }
}
