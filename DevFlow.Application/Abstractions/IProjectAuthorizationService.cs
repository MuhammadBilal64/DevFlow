namespace DevFlow.Application.Abstractions
{
    public interface IProjectAuthorizationService
    {
        Task EnsureCanManageProjectAsync(int projectId);

        Task EnsureProjectMemberAsync(int projectId);

        Task EnsureProjectMemberAsync(int projectId, int userId);
    }
}
