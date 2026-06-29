using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.GetTaskById;
using MediatR;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectHandler : IRequestHandler<GetTasksByProjectQuery, PagedResult<GetTasksByProjectResult>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        public GetTasksByProjectHandler(IProjectRepository projectRepository,ITaskRepository taskRepository,IWorkspaceAuthorizationService workspaceAuthorizationService)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
        }
        public async Task<PagedResult<GetTasksByProjectResult>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            var project=await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("Project Doesnot Exist");
            }
            await _workspaceAuthorizationService.EnsureWorkspaceMemberAsync(project.WorkspaceId);

            var paginatedData = await _taskRepository.GetTasksByProjectAsync(
     request.ProjectId,
     request.SearchTerm,
     request.PageNumber,
     request.PageSize);
            var totalPages = (int)Math.Ceiling((double)paginatedData.TotalCount / request.PageSize);


            var tasks = paginatedData.Items.Select(i => new GetTasksByProjectResult
            {
                TaskId=i.Id,
                Title=i.Title,
                
            }).ToList();
            return new PagedResult<GetTasksByProjectResult>
            {
                Items = tasks,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = paginatedData.TotalCount,
                TotalPages = totalPages
            };
        
           
        }

    }
}
