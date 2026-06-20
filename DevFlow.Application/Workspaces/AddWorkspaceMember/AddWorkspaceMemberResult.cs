using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workspaces.AddWorkspaceMember
{
    public class AddWorkspaceMemberResult
    {
        public int UserId {  get; set; }
        public int WorkspaceId {  get; set; }
        public WorkspaceRole Role { get; set; }

    }
}
