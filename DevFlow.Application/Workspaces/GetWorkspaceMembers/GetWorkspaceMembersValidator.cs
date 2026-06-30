using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersValidator:AbstractValidator<GetWorkspaceMembersQuery>
    {
        public GetWorkspaceMembersValidator() {
            Include(new PaginationRequestValidator());
        }
    }
}
