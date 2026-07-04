using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Events;

namespace DevFlow.Domain.Entities
{
    public class Project:BaseEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public int CreatedBy {  get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int WorkspaceId {  get; private set; }
        public Workspace Workspace { get; private set; } = null!;
        public User Creator { get; private set; } = null!;
        public ICollection<TaskItem> Tasks { get;private set; }= new List<TaskItem>();
        public Project(
            string name,
            string description,
            int workspaceId,
            int createdBy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            if (workspaceId <= 0)
                throw new ArgumentException("Invalid workspace id.", nameof(workspaceId));

            if (createdBy <= 0)
                throw new ArgumentException("Invalid creator id.", nameof(createdBy));
            if(string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Project description is required.", nameof(description));


            Name = name;
            Description = description;
            WorkspaceId = workspaceId;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            AddDomainEvent(new ProjectCreatedEvent(
    Name,
    WorkspaceId,
    CreatedBy));
        }
        private Project()
        {
        }
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            if (Name == name)
                return;

            Name = name;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Project description is required.", nameof(description));

            if (Description == description)
                return;

            Description = description;
        }


    }
}
