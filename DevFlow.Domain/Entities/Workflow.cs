using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class Workflow
    {
        public int Id { get;private set;  }
        public string Name { get; private set; } = null!;
        public string ?Description { get; private set; } 
        public WorkflowTrigger Trigger {  get; private set; } 
        public bool IsEnabled {  get; private set; }=false;
        public DateTime CreatedAt { get; private set; }
        public DateTime ?UpdatedAt {  get; private set; }
        public ICollection<WorkflowCondition> Conditions { get; private set; } = new List<WorkflowCondition>();
        public ICollection<WorkflowAction> Actions { get; private set; } = new List<WorkflowAction>();
        public Workflow(string name, string? description, WorkflowTrigger trigger)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workflow name is required.", nameof(name));

            Name = name;
            Description = description;
            Trigger = trigger;
            CreatedAt = DateTime.UtcNow;
          
           
        }
        public void Enable()
        {
            if (IsEnabled)
                return;
            IsEnabled = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Disable()
        {
            if (!IsEnabled)
                return;
            IsEnabled = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workflow name is required.", nameof(name));

            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        private Workflow() { }
        public void AddAction(WorkflowAction workflowAction)
        {
            if (workflowAction == null)
                throw new ArgumentNullException(nameof(workflowAction));
            if (IsEnabled) throw new InvalidOperationException(
            "Cannot modify an enabled workflow.");
            Actions.Add(workflowAction);
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveAction(WorkflowAction workflowAction)
        {
            if (workflowAction == null)
                throw new ArgumentNullException(nameof(workflowAction));
            if (IsEnabled)
                throw new InvalidOperationException("Cannot modify an enabled workflow.");
            if (Actions.Remove(workflowAction))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public void AddCondtion(WorkflowCondition workflowCondition)
        {
            if (workflowCondition == null)
                throw new ArgumentNullException(nameof(workflowCondition));
            if (IsEnabled) throw new InvalidOperationException("cannot modify an enabled workflow");
            Conditions.Add(workflowCondition);
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveCondtion(WorkflowCondition workflowCondition)
        {
            if (workflowCondition == null)
                throw new ArgumentNullException(nameof(workflowCondition));
            if (IsEnabled)
                throw new InvalidOperationException("Cannot modify an enabled workflow.");
            if (Conditions.Remove(workflowCondition))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
