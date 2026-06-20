using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaceById
{
    public class GetWorkspaceByIdHandler : IRequestHandler<GetWorkspaceByIdQuery, GetWorkspaceByIdResult>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        public GetWorkspaceByIdHandler(IWorkspaceRepository workspaceRepository) { 
            _workspaceRepository = workspaceRepository;
        }
        public async Task<GetWorkspaceByIdResult> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
        {
            var workspace =  await _workspaceRepository.GetByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace doesnt exist");
            }
            var result = new GetWorkspaceByIdResult
            {
                Id = workspace.Id,
                Name = workspace.Name,
                CreatedAt = workspace.CreatedAt
            };
            return result;

        }
    }
}
