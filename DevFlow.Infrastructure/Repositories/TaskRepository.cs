using System.Linq.Expressions;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DevFlowDbContext _context;
        public TaskRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }


        public async Task<TaskItem?> GetByIdAsync(int TaskId)
        {
            return await _context.Tasks.Include(p => p.Project).FirstOrDefaultAsync(i => i.Id == TaskId);
        }

        public async Task<PaginatedData<TaskItem>> GetTasksByProjectAsync(int projectId, string? searchTerm, string? sortBy,
    bool descending, Domain.Enum.TaskStatus? status, TaskPriority? priority, int pageNumber, int pageSize)
        {
            var query = _context.Tasks.AsNoTracking().
                Where(i => i.ProjectId == projectId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(searchTerm) ||
                    t.Description.Contains(searchTerm));
            }
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);

            }
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }
            var sortingFields = new Dictionary<string, Expression<Func<TaskItem, object>>>{
            { "title", t => t.Title },
            { "createdat", t => t.CreatedAt },
             { "priority", t => t.Priority },
             { "status", t => t.Status }
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
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = new PaginatedData<TaskItem>
            {
                Items = items,
                TotalCount = totalCount,
            };
            return result;
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);

        }


    }
}
