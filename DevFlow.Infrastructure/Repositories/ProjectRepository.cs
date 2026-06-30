using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DevFlowDbContext _context;
        public ProjectRepository(DevFlowDbContext context)
        {
            _context= context;
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.
                AddAsync(project);
        }

        public async Task<bool> ExistsInWorkspaceAsync(int workspaceId, string projectName)
        {
            return await _context.Projects.AnyAsync(u => u.WorkspaceId == workspaceId && u.Name == projectName);
           
        }

        public async Task<Project?> GetByIdAsync(int Id)
        {
            return await _context.Projects.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<PaginatedData<Project>> GetProjectsByWorkspaceAsync(int workspaceId,string ? searchTerm, string? sortBy,
    bool descending, int pageNumber,
            int pageSize)
        {
            var query =  _context.Projects.AsNoTracking().Where(u => u.WorkspaceId == workspaceId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }
            var sortingFields = new Dictionary<string, Expression<Func<Project, object>>>
            {
                {"name", p=>p.Name},{"createdat",p=>p.CreatedAt}
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
            var result = new PaginatedData<Project>
            {
                Items = items,
                TotalCount = totalCount,
            };
            return result;
        }

        public async Task UpdateAsync(Project project)
        {
             _context.Projects.Update(project);
        }
    }
}
