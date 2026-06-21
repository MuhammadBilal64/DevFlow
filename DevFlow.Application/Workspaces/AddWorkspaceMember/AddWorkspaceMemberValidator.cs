using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DevFlow.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberValidator:AbstractValidator<AddWorkspaceMemberCommand>
    {

        public AddWorkspaceMemberValidator()
        {
            RuleFor(u => u.UserId).GreaterThan(0);
            RuleFor(u => u.WorkspaceId).GreaterThan(0);
            RuleFor(U => U.Role).IsInEnum();
        }
    }
}
