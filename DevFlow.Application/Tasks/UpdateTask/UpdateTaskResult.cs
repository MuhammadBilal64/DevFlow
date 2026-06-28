using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Tasks.UpdateTask
{
    public class UpdateTaskResult
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
