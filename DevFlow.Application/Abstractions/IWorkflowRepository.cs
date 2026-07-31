using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkflowRepository
    {
        Task AddAsync(Workflow workflow);

        Task<Workflow?> GetByIdAsync(int workflowId);

        Task UpdateAsync(Workflow workflow);

        Task DeleteAsync(Workflow workflow);

        Task<bool> ExistsInProjectAsync(
            int projectId,
            string workflowName);

        Task<IReadOnlyList<Workflow>> GetEnabledByTriggerAsync(
            int projectId,
            WorkflowTrigger trigger);

        Task<PaginatedData<Workflow>> GetByProjectAsync(
            int projectId,
            string? searchTerm,
            WorkflowTrigger? trigger,
            bool? isEnabled,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
    }
}
