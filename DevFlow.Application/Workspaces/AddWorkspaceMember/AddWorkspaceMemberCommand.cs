using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberCommand:IRequest<AddWorkspaceMemberResult>
    {
        public int WorkspaceId {  get; set; }
        public int UserId {  get; set; }
        public WorkspaceRole Role { get; set; }
    }
}
