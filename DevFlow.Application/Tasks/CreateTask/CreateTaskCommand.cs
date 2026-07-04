using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;
using MediatR;

namespace DevFlow.Application.Tasks.CreateTask
{
    public class CreateTaskCommand:IRequest<CreateTaskResult>
    {
        public int ProjectId {  get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
     
    }
}
