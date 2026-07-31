using MediatR;

namespace DevFlow.Application.Tasks.UpdateTaskStatus
{
    public class UpdateTaskStatusCommand : IRequest<UpdateTaskStatusResult>
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }

        public DevFlow.Domain.Enum.TaskStatus TaskStatus { get; set; }
    }
}
