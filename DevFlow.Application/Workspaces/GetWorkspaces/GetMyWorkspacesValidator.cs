using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesValidator:AbstractValidator<GetMyWorkspacesQuery>
    {
        public GetMyWorkspacesValidator() {
            Include(new PaginationRequestValidator());
        }
    }
}
