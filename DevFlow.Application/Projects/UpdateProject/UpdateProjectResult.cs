using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Projects.UpdateProject
{
    public class UpdateProjectResult
    {
       public int Id {  get; set; }
        public string ProjectName { get; set; } = null!;
        public string Description {  get; set; } = null!;
    }
}
