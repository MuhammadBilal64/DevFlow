namespace DevFlow.Application.ProjectMembers.GetProjectMembers
{
    public class GetProjectMembersResult
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public DateTime JoinedAt { get; set; }
    }
}
