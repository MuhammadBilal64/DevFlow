using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.GetTaskById;
using MediatR;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectHandler : IRequestHandler<GetTasksByProjectQuery, List<GetTasksByProjectResult>>
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
        public async Task<List<GetTasksByProjectResult>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            var project=await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                throw new NotFoundException("Project Doesnot Exist");
            }
            await _workspaceAuthorizationService.EnsureWorkspaceMemberAsync(project.WorkspaceId);
            var result = await _taskRepository.GetTasksByProjectAsync(request.ProjectId);
            var projects = result.Select(i => new GetTasksByProjectResult
            {
                TaskId=i.Id,
                Title=i.Title,
                
            }).ToList();
            return projects;
           
        }

    }
}
