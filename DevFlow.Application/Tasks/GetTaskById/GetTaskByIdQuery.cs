using MediatR;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdQuery : IRequest<GetTaskByIdResult>
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }


    }
}
