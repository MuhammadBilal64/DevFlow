using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Projects.GetProjectById
{
    public class GetProjectByIdQuery:IRequest<GetProjectByIdResult>
    {
        public int Id { get; set; }
    }
}
