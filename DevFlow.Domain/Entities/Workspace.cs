using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Domain.Entities
{
    public class Workspace
    {
        public int Id {  get; set; }
        public string Name { get; set; } = null!;
        public int CreatedBy {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public User? Creator {  get; set; }
        public ICollection<WorkspaceMember> Members { get; set; } =new List<WorkspaceMember>();


    }
}
