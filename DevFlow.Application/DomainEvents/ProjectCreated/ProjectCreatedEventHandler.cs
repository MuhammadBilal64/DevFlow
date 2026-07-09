using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.ProjectCreated
{
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IWorkflowEngine _workflowEngine;

        public ProjectCreatedEventHandler(
            INotificationService notificationService,
            IWorkspaceMemberRepository workspaceMemberRepository,
            IWorkflowEngine workflowEngine)
        {
            _notificationService = notificationService;
            _workspaceMemberRepository = workspaceMemberRepository;
            _workflowEngine = workflowEngine;
        }

        public  async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            
            var members = await _workspaceMemberRepository.GetAllByWorkspaceIdAsync(notification.WorkspaceId);
            
            foreach (var member in members)
            {
                if (member.UserId == notification.CreatedBy)
                    continue;
                await _notificationService.NotifyAsync(
                                   member.UserId,
                                   $"Project '{notification.ProjectName}' was created.",
                                   NotificationType.ProjectCreated);
                var values = new Dictionary<string, object?>
                {
                    { "ProjectName", notification.ProjectName },
                    { "WorkspaceId", notification.WorkspaceId },
                    { "CreatorId", notification.CreatedBy },
                    { "MemberId", member.UserId }
                };

                var context = new WorkflowExecutionContext(values);

                await _workflowEngine.ExecuteAsync(
                    WorkflowTrigger.ProjectCreated,
                    context);

            }
           

        }
    }
}
