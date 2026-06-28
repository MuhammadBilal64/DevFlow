using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskResult
    {
        public int TaskId {  get; set; }
        public string Title { get; set; } = null!;
    }
}
