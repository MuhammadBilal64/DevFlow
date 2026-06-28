using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DevFlow.Application.Tasks.CreateTask
{
    public class CreateTaskResult
    {
        public int TaskId {  get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

    }
}
