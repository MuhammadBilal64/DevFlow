namespace DevFlow.Domain.Events
{
    public class TaskCompletedEvent : IDomainEvent
    {
        public int RecipientUserId { get; }
        public int TaskId { get; }
        public string TaskTitle { get; } = null!;
        public int ProjectId { get; }
        public TaskCompletedEvent(int userId, int taskId, string taskTitle, int projectId)
        {

            RecipientUserId = userId;
            TaskId = taskId;
            TaskTitle = taskTitle;
            ProjectId = projectId;
        }

    }
}
