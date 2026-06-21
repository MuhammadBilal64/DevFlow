using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Infrastructure.Persistence;

namespace DevFlow.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DevFlowDbContext _context;
        public UnitOfWork(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
          await  _context.SaveChangesAsync();
        }
    }
}
