using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdResult
    {
        public int TaskId {  get; set; }
        public string Title { get; set } = null!;
        public string Description { get; set; } = null!;
    }
}
