using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceResult
    {
        public int ProjectId {  get; set; }
        public string ProjectName { get; set; } = null!;
        public string Description {  get; set; } = null!;
    }
}
