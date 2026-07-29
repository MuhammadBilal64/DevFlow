using DevFlow.Application.Workspaces.RemoveWorkspaceMember;
using FluentValidation;

public class RemoveWorkspaceMemberValidator
    : AbstractValidator<RemoveWorkspaceMemberCommand>
{
    public RemoveWorkspaceMemberValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.WorkspaceId)
            .GreaterThan(0);
    }
}