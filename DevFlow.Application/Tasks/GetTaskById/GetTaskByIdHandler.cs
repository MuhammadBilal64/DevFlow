using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, GetTaskByIdResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IWorkspaceAuthorizationService _authorizationService;
        public GetTaskByIdHandler( ITaskRepository taskRepository,IWorkspaceAuthorizationService service)
        {

            _taskRepository = taskRepository;
            _authorizationService = service;
        }
            
        public async Task<GetTaskByIdResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task not Exist");
            }
            await _authorizationService.EnsureWorkspaceMemberAsync(task.Project.WorkspaceId);
            var result = new GetTaskByIdResult
            {
                TaskId=task.Id,
                Title=task.Title,
                Description=task.Description,
            };
            return result;
        }
    }
}
