using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeCommand:IRequest<UpdateTaskAssigneeResult>
    {
        public int TaskId {  get; set; }
        public int NewAssigneeId {  get; set; }

    }
}
