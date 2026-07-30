using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IProjectMemberRepository
    {
        Task AddAsync(ProjectMember member);

        Task<bool> IsMemberAsync(
            int projectId,
            int userId);

        Task<List<ProjectMember>> GetAllByProjectIdAsync(
            int projectId);

        Task<ProjectMember?> GetMemberAsync(
            int projectId,
            int userId);

        Task RemoveAsync(ProjectMember member);
    }
}
