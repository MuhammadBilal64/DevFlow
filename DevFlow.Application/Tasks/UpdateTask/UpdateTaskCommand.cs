using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Tasks.UpdateTask
{
    public class UpdateTaskCommand:IRequest<UpdateTaskResult>
    {
        public int TaskId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public TaskPriority Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
