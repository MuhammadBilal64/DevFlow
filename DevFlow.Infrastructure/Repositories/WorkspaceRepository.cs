using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Workspace> GetByIdAsync(int workspaceId)
        {
           var workspace= await _context.Workspaces.FirstOrDefaultAsync(u=>u.Id == workspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace does not exist");
            }
            return workspace;
        }
    }
}
