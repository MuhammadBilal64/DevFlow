using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workspaces.RemoveWorkspaceMember
{
    public class RemoveWorkspaceMemberCommand:IRequest<RemoveWorkspaceMemberResult>
    {
        public int UserId { get; set; }
        public int WorkspaceId {  get; set; }
    }
}
