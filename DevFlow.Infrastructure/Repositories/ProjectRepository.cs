using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
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

        public async Task<List<Project>> GetProjectsByWorkspaceAsync(int workspaceId)
        {
            var result = await _context.Projects.Where(u => u.WorkspaceId == workspaceId).ToListAsync();
            return result;
        }

        public async Task UpdateAsync(Project project)
        {
             _context.Projects.Update(project);
        }
    }
}
