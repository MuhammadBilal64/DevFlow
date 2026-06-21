using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Abstractions
{
    public interface IWorkspaceRepository
    {
        Task AddAsync(Workspace workspace );
        Task <Workspace?>GetByIdAsync(int workspaceId);

    }
}
