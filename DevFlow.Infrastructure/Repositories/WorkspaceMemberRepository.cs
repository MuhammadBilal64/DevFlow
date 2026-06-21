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
    public class WorkspaceMemberRepository : IWorkspaceMemberRepository
    {
        private readonly DevFlowDbContext _context;
        public WorkspaceMemberRepository(DevFlowDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(WorkspaceMember member)
        {
             await _context.WorkspacesMembers.AddAsync(member);

          
        }

        public async Task<List<WorkspaceMember>> GetAllMembersAsync(int workspaceId)
        {
            return await _context.WorkspacesMembers.Include(u=>u.User).Where(u=>u.WorkspaceId==workspaceId) .ToListAsync();
        }

        public Task<List<WorkspaceMember>> GetByUserIdAsync(int userId)
        {
            var result =  _context.WorkspacesMembers.Include(u => u.Workspace).Where(u => u.UserId == userId).ToListAsync();
            return result;
        }

        public async Task<WorkspaceMember?> GetMemberAsync(int userId, int workspaceId)
        {
            return await _context.WorkspacesMembers
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.WorkspaceId == workspaceId);
        }

        public async Task RemoveAsync(int userId, int WorkspaceId)
        {
            var member= await _context.WorkspacesMembers.FirstOrDefaultAsync(x => x.UserId == userId && x.WorkspaceId == WorkspaceId);
            if (member == null)
            {
                return;
            }
          _context.WorkspacesMembers.Remove(member);

        }
    }
}
