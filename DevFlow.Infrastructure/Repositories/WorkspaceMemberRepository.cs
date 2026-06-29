using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
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

      
        public async Task<PaginatedData<WorkspaceMember>> GetAllMembersAsync(int workspaceId, int pageNumber, int pageSize)
        {
            var query = _context.WorkspacesMembers.AsNoTracking().Include(u => u.User).Where(u => u.WorkspaceId == workspaceId);
            var totalCount=await query.CountAsync();
            var items=await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            var result = new PaginatedData<WorkspaceMember>
            {
                Items= items,
                TotalCount=totalCount
            };
            return result;
        }

        public Task<List<WorkspaceMember>> GetByUserIdAsync(int userId)
        {
            var result =  _context.WorkspacesMembers.Include(u => u.Workspace).Where(u => u.UserId == userId).ToListAsync();
            return result;
        }

        public async Task<PaginatedData<WorkspaceMember>> GetByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            var query=_context.WorkspacesMembers.AsNoTracking().Include(w=>w.Workspace).Where(i=>i.UserId==userId);
            var totalCount=await query.CountAsync();
            var items = await query.Skip((pageNumber - 1 )* pageSize).Take(pageSize).ToListAsync();
            var result = new PaginatedData<WorkspaceMember>
            {
                Items=items,
                TotalCount=totalCount

            };
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
