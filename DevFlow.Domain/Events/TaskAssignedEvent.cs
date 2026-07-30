namespace DevFlow.Domain.Events
{
    public sealed class TaskAssignedEvent : IDomainEvent
    {
        public int UserId { get; }
        public int TaskId { get; }
        public int ProjectId { get; }
        public string TaskTitle { get; } = null!;

        public TaskAssignedEvent(
            int userId,
            int taskId,
            int projectId,
            string taskTitle)
        {
            UserId = userId;
            TaskId = taskId;
            ProjectId = projectId;
            TaskTitle = taskTitle;
        }
    }
}