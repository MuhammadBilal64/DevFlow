using System.Linq.Expressions;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
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

        public async Task<PaginatedData<Workflow>> GetAllAsync(
     string? searchTerm,
     WorkflowTrigger? trigger,
     bool? isEnabled,
     string? sortBy,
     bool descending,
     int pageNumber,
     int pageSize)
        {
            var query = _context.Workflows
                .AsNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(w =>
                    w.Name.Contains(searchTerm) ||
                    (w.Description != null &&
                     w.Description.Contains(searchTerm)));
            }

            // Trigger filter
            if (trigger.HasValue)
            {
                query = query.Where(w => w.Trigger == trigger.Value);
            }

            // Enabled filter
            if (isEnabled.HasValue)
            {
                query = query.Where(w => w.IsEnabled == isEnabled.Value);
            }

            // Sorting

            var sortingFields =
                new Dictionary<string, Expression<Func<Workflow, object>>>
                {
            { "name", w => w.Name },
            { "createdat", w => w.CreatedAt },
            { "trigger", w => w.Trigger },
            { "isenabled", w => w.IsEnabled }
                };

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortingFields.TryGetValue(sortBy.ToLower(), out var expression))
                {
                    query = descending
                        ? query.OrderByDescending(expression)
                        : query.OrderBy(expression);
                }
                else
                {
                    query = query.OrderByDescending(w => w.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(w => w.CreatedAt);
            }

            // Pagination

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedData<Workflow>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}