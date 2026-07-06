using System;
using System.Collections.Generic;
using System.Text;
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

        Task<IReadOnlyList<Workflow>> GetActiveByTriggerAsync(
            WorkflowTrigger trigger);
    }
}
