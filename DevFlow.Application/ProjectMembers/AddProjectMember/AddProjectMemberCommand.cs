using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.ProjectMembers.AddProjectMember
{
    public class AddProjectMemberCommand : IRequest<AddProjectMemberResult>
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public ProjectRole Role { get; set; }
    }
}
