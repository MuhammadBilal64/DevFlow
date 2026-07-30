using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.GetAllWorkflows;
using DevFlow.Domain.Enum;
using MediatR;

public class GetWorkflowsByProjectQuery
    : PaginationRequest,
      IRequest<PagedResult<GetWorkflowsByProjectResult>>
{
    public int ProjectId { get; set; }

    public WorkflowTrigger? Trigger { get; set; }

    public bool? IsEnabled { get; set; }
}