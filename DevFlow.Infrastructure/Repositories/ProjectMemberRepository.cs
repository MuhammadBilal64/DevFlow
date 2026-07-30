using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly DevFlowDbContext _context;
        public ProjectMemberRepository(DevFlowDbContext context)
        {
            _context = context;

        }

        public async Task AddAsync(ProjectMember member)
        {
            await _context.ProjectMembers.AddAsync(member);
        }

        public async Task<List<ProjectMember>> GetAllByProjectIdAsync(int projectId)
        {
            return await _context.ProjectMembers.Where(i => i.ProjectId == projectId).ToListAsync();
        }

        public async Task<ProjectMember?> GetMemberAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers.FirstOrDefaultAsync(i => i.ProjectId == projectId && i.UserId == userId);
        }

        public async Task<bool> IsMemberAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers.AnyAsync(i => i.ProjectId == projectId && i.UserId == userId);
        }

        public async Task RemoveAsync(ProjectMember member)
        {
            _context.ProjectMembers.Remove(member);
        }
    }
}
