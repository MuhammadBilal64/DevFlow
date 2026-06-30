using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceValidator:AbstractValidator<GetProjectsByWorkspaceQuery>
    {
        public GetProjectsByWorkspaceValidator() { Include(new PaginationRequestValidator()); }
    }
}
