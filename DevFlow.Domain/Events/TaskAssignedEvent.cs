using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Domain.Events
{
    public sealed class TaskAssignedEvent:IDomainEvent
    {
        public int UserId {  get;  }
        public int TaskId {  get;  }
        public string TaskTitle { get; } = null!;
        public TaskAssignedEvent(int userId, int taskId, string taskTitle)
        {
            UserId = userId;
            TaskId = taskId;
            TaskTitle = taskTitle;
        }
    }
}
