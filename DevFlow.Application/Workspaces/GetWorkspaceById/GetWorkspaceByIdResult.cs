using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Workspaces.GetWorkspaceById
{
    public class GetWorkspaceByIdResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt {  get; set; }
    }
}
