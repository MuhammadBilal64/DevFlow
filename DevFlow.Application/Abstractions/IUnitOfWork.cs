using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Abstractions
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
