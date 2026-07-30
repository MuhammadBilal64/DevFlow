using FluentValidation;

namespace DevFlow.Application.ProjectMembers.RemoveProjectMember
{
    public class RemoveProjectMemberValidator
       : AbstractValidator<RemoveProjectMemberCommand>
    {
        public RemoveProjectMemberValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
