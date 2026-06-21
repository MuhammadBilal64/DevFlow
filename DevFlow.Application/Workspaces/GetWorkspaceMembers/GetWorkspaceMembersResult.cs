using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersResult
    {
        public int UserId {  get; set; }
        public string Name { get; set; } = null!;
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }


    }
}
