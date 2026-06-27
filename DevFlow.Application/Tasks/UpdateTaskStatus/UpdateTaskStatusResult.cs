using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusResult
    {
        public int Id {  get; set; }
        public DevFlow.Domain.Enum.TaskStatus Status { get; set; }
    }
}
