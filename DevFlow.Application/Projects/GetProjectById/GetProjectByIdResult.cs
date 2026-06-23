using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Projects.GetProjectById
{
    public class GetProjectByIdResult
    {
        public string Description { get; set; } = null!;
        public int ProjectId {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public String ProjectName { get; set; } = null!;

    }
}
