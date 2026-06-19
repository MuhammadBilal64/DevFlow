using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesHandler : IRequestHandler<GetMyWorkspacesQuery, List<GetMyWorkspacesResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

        public GetMyWorkspacesHandler(ICurrentUserService currentUserService,IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;

        }

        public async Task<List<GetMyWorkspacesResult>> Handle(GetMyWorkspacesQuery request, CancellationToken cancellationToken)
        {
            var userId=_currentUserService.UserId;
           var memberships=await _workspaceMemberRepository.GetByUserIdAsync(userId);
            var result = memberships.Select(u => new GetMyWorkspacesResult
            {
                Id=u.Workspace!.Id,
                Name=u.Workspace.Name
            }).ToList();
            return result;
        }
    }
}
