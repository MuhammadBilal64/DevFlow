using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Tasks.GetTaskById
{
    public class GetTaskByIdQuery:IRequest<GetTaskByIdResult>
    {
        public int TaskId {  get; set; }
        
    }
}
