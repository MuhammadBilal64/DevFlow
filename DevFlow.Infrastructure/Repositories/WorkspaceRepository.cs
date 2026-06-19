using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;

namespace DevFlow.Infrastructure.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly DevFlowDbContext _context;
        public WorkspaceRepository(DevFlowDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Workspace workspace)
        {
             await _context.Workspaces.AddAsync(workspace);
           await _context.SaveChangesAsync();
        }
    }
}
