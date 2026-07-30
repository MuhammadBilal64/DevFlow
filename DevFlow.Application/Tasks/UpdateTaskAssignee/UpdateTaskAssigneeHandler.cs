using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeHandler : IRequestHandler<UpdateTaskAssigneeCommand, UpdateTaskAssigneeResult>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        public UpdateTaskAssigneeHandler(
    IUnitOfWork unitOfWork,
    ITaskRepository taskRepository,
    IProjectAuthorizationService projectAuthorizationService)
        {
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<UpdateTaskAssigneeResult> Handle(UpdateTaskAssigneeCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository
                             .GetByIdAsync(request.TaskId);

            if (task == null)
            {
                throw new NotFoundException("Task Doesnot Exist");
            }

            await _projectAuthorizationService
                                      .EnsureProjectMemberAsync(task.ProjectId);


            if (request.NewAssigneeId != null)
            {
                await _projectAuthorizationService
                                            .EnsureProjectMemberAsync(
        task.ProjectId,
        request.NewAssigneeId.Value);
                task.Assign(request.NewAssigneeId.Value);


            }
            else
            {
                task.Unassign();
            }

            await _taskRepository.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
            var result = new UpdateTaskAssigneeResult
            {
                Id = task.Id,
                Title = task.Title,
            };
            return result;
        }
    }
}
