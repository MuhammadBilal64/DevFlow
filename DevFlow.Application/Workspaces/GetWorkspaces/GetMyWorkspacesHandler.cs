using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesHandler : IRequestHandler<GetMyWorkspacesQuery, PagedResult<GetMyWorkspacesResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

        public GetMyWorkspacesHandler(ICurrentUserService currentUserService, IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _currentUserService = currentUserService;
            _workspaceMemberRepository = workspaceMemberRepository;

        }

        public async Task<PagedResult<GetMyWorkspacesResult>> Handle(GetMyWorkspacesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
           
            var paginatedWorkspaces = await _workspaceMemberRepository.GetByUserIdAsync(userId,request.SearchTerm,request.SortBy,request.Descending,request.PageNumber, request.PageSize);
            var totalPages = (int)Math.Ceiling((double)paginatedWorkspaces.TotalCount / request.PageSize);
            var result = paginatedWorkspaces.Items.Select(u => new GetMyWorkspacesResult
            {
                Id = u.Workspace!.Id,
                Name = u.Workspace.Name
            }).ToList();
            return new PagedResult<GetMyWorkspacesResult>
            {
                Items = result,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = paginatedWorkspaces.TotalCount,
                TotalPages = totalPages
            };

        }
    }
}
