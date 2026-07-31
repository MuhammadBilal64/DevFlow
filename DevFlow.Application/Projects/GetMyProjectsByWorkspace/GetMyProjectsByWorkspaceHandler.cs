using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceHandler
        : IRequestHandler<GetMyProjectsByWorkspaceQuery, PagedResult<GetMyProjectsByWorkspaceResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectRepository _projectRepository;

        public GetMyProjectsByWorkspaceHandler(
            ICurrentUserService currentUserService,
            IProjectRepository projectRepository)
        {
            _currentUserService = currentUserService;
            _projectRepository = projectRepository;
        }

        public async Task<PagedResult<GetMyProjectsByWorkspaceResult>> Handle(
            GetMyProjectsByWorkspaceQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var projects = await _projectRepository.GetProjectsForUserAsync(
                request.WorkspaceId,
                userId,
                request.SearchTerm,
                request.SortBy,
                request.Descending,
                request.PageNumber,
                request.PageSize);

            var totalPages = (int)Math.Ceiling(
                (double)projects.TotalCount / request.PageSize);

            var result = projects.Items.Select(x => new GetMyProjectsByWorkspaceResult
            {
                ProjectId = x.Id,
                ProjectName = x.Name,
                Description = x.Description,
            }).ToList();

            return new PagedResult<GetMyProjectsByWorkspaceResult>
            {
                Items = result,
                TotalCount = projects.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
    }
}