using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class ProjectMember
    {
        public int Id { get; private set; }

        public int ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        public int UserId { get; private set; }
        public User User { get; private set; } = null!;

        public ProjectRole Role { get; private set; }

        public DateTime JoinedAt { get; private set; }

        ProjectMember(
    Project project,
    int userId,
    ProjectRole role)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (userId <= 0)
                throw new ArgumentException(
                    "Invalid user id.",
                    nameof(userId));

            Project = project;
            UserId = userId;
            Role = role;
            JoinedAt = DateTime.UtcNow;
        }

        private ProjectMember()
        {
        }
    }
}