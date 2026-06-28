using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using TaskStatus = DevFlow.Domain.Enum.TaskStatus;

namespace DevFlow.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int CreatedBy { get; set; }
        public User Creator { get; set; } = null!;

        public int? AssignedToUserId { get; set; }
        public User? Assignee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
    }

}
