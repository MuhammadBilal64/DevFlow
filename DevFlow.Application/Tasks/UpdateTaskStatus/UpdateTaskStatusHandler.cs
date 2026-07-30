using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, UpdateTaskStatusResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public UpdateTaskStatusHandler(
       ITaskRepository taskRepository,
       IUnitOfWork unitOfWork,
       IProjectAuthorizationService projectAuthorizationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _projectAuthorizationService = projectAuthorizationService;
        }
        public async Task<UpdateTaskStatusResult> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository
                                            .GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }
            await _projectAuthorizationService
                                    .EnsureProjectMemberAsync(task.ProjectId);

            task.UpdateStatus(request.TaskStatus);

            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateTaskStatusResult
            {

                Id = task.Id,
                Status = task.Status
            };
        }
    }
}
