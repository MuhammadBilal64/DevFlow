namespace DevFlow.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceResult
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}