using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Models;
using MediatR;

namespace DevFlow.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectQuery:PaginationRequest,IRequest<PagedResult<GetTasksByProjectResult>>
    {
        public int ProjectId { get; set; }
    }
}
