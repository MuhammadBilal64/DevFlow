using FluentValidation;

namespace DevFlow.Application.ProjectMembers.AddProjectMember
{

    public class AddProjectMemberValidator
        : AbstractValidator<AddProjectMemberCommand>
    {
        public AddProjectMemberValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.Role)
                .IsInEnum();
        }
    }
}
