using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using TaskStatus = DevFlow.Domain.Enum.TaskStatus;

namespace DevFlow.Domain.Entities
{
    public class TaskItem:BaseEntity
    {
        public int Id { get; private set; }

        public string Title { get; private set; } = null!;
        public string? Description { get; private set; } 

        public int ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        public int CreatedBy { get; private set; }
        public User Creator { get; private set; } = null!;

        public int? AssignedToUserId { get; private set; }
        public User? Assignee { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? AssignedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public TaskPriority Priority { get; private set; }
        public TaskStatus Status { get; private set; }

        public TaskItem(
            string title,
            string? description,
            int projectId,
            int createdBy,
            DateTime? dueDate,
            TaskPriority priority)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title is required.", nameof(title));

            if (projectId <= 0)
                throw new ArgumentException("Invalid project id.", nameof(projectId));

            if (createdBy <= 0)
                throw new ArgumentException("Invalid creator id.", nameof(createdBy));

            if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
                throw new ArgumentException("Due date cannot be in the past.", nameof(dueDate));

            Title = title;
            Description = description;
            ProjectId = projectId;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            Priority = priority;
            Status = TaskStatus.Todo;
        }
        private TaskItem()
        {

        }
        public void Assign(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid User Id",nameof(userId));
            if(AssignedToUserId== userId)
            {
                return;
            }
            AssignedToUserId = userId;
            AssignedAt= DateTime.UtcNow;
            AddDomainEvent(new TaskAssignedEvent(userId,Id,Title));
        }
        public void Unassign()
        {
            if (AssignedToUserId == null)
                return;

            AssignedToUserId = null;
            AssignedAt = null;
        }
        public void UpdateStatus(TaskStatus status)
        {
            if(Status == status)
            {
                return;
            }
            Status= status;
            if (status == TaskStatus.Completed)
            {
                CompletedAt= DateTime.UtcNow;
                AddDomainEvent(new TaskCompletedEvent(CreatedBy,Id,Title));
            }
            else
            {
                CompletedAt = null;
            }
        }
        public void UpdatePriority(TaskPriority priority)
        {
            Priority = priority;
        }
        public void UpdateDueDate(DateTime? dueDate)
        {
            if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
                throw new ArgumentException("Due date cannot be in the past.", nameof(dueDate));

            DueDate = dueDate;
        }
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title is required.", nameof(title));

            if (Title == title)
                return;

            Title = title;
        }

        public void UpdateDescription(string? description)
        {
            if (Description == description)
                return;

            Description = description;
        }

    }

}
