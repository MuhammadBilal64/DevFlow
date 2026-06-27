using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Tasks.DeleteTask
{
    public class DeleteTaskCommand:IRequest<DeleteTaskResult>
    {
        public int TaskId {  get; set; }
    }
}
