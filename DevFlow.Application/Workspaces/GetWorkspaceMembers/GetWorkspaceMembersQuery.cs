using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersQuery:IRequest<List<GetWorkspaceMembersResult>>
    {
        public int WorkspaceId {  get; set; }
    }
}
