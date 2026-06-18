using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class WorkspaceMember
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public int UserId {  get; set; }
        public User ?User { get; set; }
        public Workspace ? Workspace { get; set; }
        public WorkspaceRole Role {  get; set; }
        public DateTime JoinedAt {  get; set; }

    }
}
