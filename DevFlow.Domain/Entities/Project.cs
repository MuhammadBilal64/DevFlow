using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CreatedBy {  get; set; }
        public DateTime CreatedAt { get; set; }
        public int WorkspaceId {  get; set; }
        public Workspace Workspace { get; set; } = null!;
        public User Creator { get; set; } = null!;



    }
}
