using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class Workflow
    {
        public int Id { get; private set; }

        public int ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        public WorkflowTrigger Trigger { get; private set; }

        public bool IsEnabled { get; private set; }

        public int CreatedBy { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public ICollection<WorkflowCondition> Conditions { get; private set; }
            = new List<WorkflowCondition>();

        public ICollection<WorkflowAction> Actions { get; private set; }
            = new List<WorkflowAction>();

        public Workflow(
            Project project,
            int createdBy,
            string name,
            string? description,
            WorkflowTrigger trigger)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (createdBy <= 0)
                throw new ArgumentException(
                    "Invalid creator id.",
                    nameof(createdBy));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Workflow name is required.",
                    nameof(name));

            Project = project;
            CreatedBy = createdBy;

            Name = name;
            Description = description;
            Trigger = trigger;

            IsEnabled = false;
            CreatedAt = DateTime.UtcNow;
        }

        private Workflow() { }

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
            EnsureCanModify();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Workflow name is required.",
                    nameof(name));

            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddCondition(WorkflowCondition condition)
        {
            EnsureCanModify();

            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            Conditions.Add(condition);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveCondition(WorkflowCondition condition)
        {
            EnsureCanModify();

            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (Conditions.Remove(condition))
                UpdatedAt = DateTime.UtcNow;
        }

        public void AddAction(WorkflowAction action)
        {
            EnsureCanModify();

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Actions.Add(action);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAction(WorkflowAction action)
        {
            EnsureCanModify();

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (Actions.Remove(action))
                UpdatedAt = DateTime.UtcNow;
        }

        private void EnsureCanModify()
        {
            if (IsEnabled)
                throw new InvalidOperationException(
                    "Cannot modify an enabled workflow.");
        }
    }
}