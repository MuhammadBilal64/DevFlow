using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Domain.Events
{
    public class TaskCompletedEvent:IDomainEvent
    {
        public int RecipientUserId { get; }
        public int TaskId { get; }
        public string TaskTitle { get; } = null!;
        public TaskCompletedEvent(int userId, int taskId, string taskTitle)
        {

            RecipientUserId = userId;
            TaskId = taskId;
            TaskTitle = taskTitle;
        }

    }
}
