using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly DevFlowDbContext _context;

        public WorkflowRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Workflow workflow)
        {
            await _context.Workflows.AddAsync(workflow);
        }

        public Task UpdateAsync(Workflow workflow)
        {
            _context.Workflows.Update(workflow);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Workflow workflow)
        {
            _context.Workflows.Remove(workflow);
            return Task.CompletedTask;
        }

        public async Task<Workflow?> GetByIdAsync(int workflowId)
        {
            return await _context.Workflows
                .Include(w => w.Conditions)
                .Include(w => w.Actions)
                .FirstOrDefaultAsync(w => w.Id == workflowId);
        }

        public async Task<IReadOnlyList<Workflow>> GetActiveByTriggerAsync(
            WorkflowTrigger trigger)
        {
            return await _context.Workflows
                .Include(w => w.Conditions)
                .Include(w => w.Actions)
                .Where(w => w.Trigger == trigger && w.IsEnabled)
                .ToListAsync();
        }
    }
}