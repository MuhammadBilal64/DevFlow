using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
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

        public  Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }

        public async Task<TaskItem?> GetByIdForAdminAsync(int taskId, int currentUserId)
        {
            return await _context.Tasks
                 .Include(i => i.Project)
                 .ThenInclude(i => i.Workspace)
                 .ThenInclude(i => i.Members)
                 .Where(i => i.Id == taskId)
                 .Where(t => t.Project.Workspace.Members
                 .Any(m => m.UserId == currentUserId && (m.Role == Domain.Enum.WorkspaceRole.Admin || (m.Role == Domain.Enum.WorkspaceRole.Owner)))).FirstOrDefaultAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int TaskId)
        {
            return await _context.Tasks.Include(p => p.Project).FirstOrDefaultAsync(i=>i.Id==TaskId);
                }

        public async Task<PaginatedData<TaskItem>> GetTasksByProjectAsync(int projectId, string? searchTerm, int pageNumber,int pageSize)
        {
            var query = _context.Tasks.AsNoTracking().
                Where(i => i.ProjectId== projectId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(searchTerm) ||
                    t.Description.Contains(searchTerm));
            }

            var totalCount=await query.CountAsync();
            var items=await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            var result = new PaginatedData<TaskItem>
            {
                Items=items,
                TotalCount=totalCount,
            };
            return result;
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
              
        }

        public async Task<TaskItem?> GetByIdForStatusUpdateAsync(int taskId, int currentUserId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Workspace)
                        .ThenInclude(w => w.Members)
                .Where(t =>
                    t.Id == taskId &&
                    (
                        t.AssignedToUserId == currentUserId ||
                        t.Project.Workspace.Members.Any(m =>
                            m.UserId == currentUserId &&
                            (m.Role == Domain.Enum.WorkspaceRole.Admin ||
                             m.Role == Domain.Enum.WorkspaceRole.Owner))
                    ))
                .FirstOrDefaultAsync();
        }
    }
}
