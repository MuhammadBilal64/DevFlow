using MediatR;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskCommand : IRequest<DeleteTaskResult>
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
    }
}
