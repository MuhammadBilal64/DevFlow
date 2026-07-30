using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, GetTaskByIdResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public GetTaskByIdHandler(ITaskRepository taskRepository, IProjectAuthorizationService service)
        {

            _taskRepository = taskRepository;
            _projectAuthorizationService = service;
        }

        public async Task<GetTaskByIdResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task not Exist");
            }
            await _projectAuthorizationService
                .EnsureProjectMemberAsync(task.ProjectId);

            var result = new GetTaskByIdResult
            {
                TaskId = task.Id,
                Title = task.Title,
                Description = task.Description,
            };
            return result;
        }
    }
}
