using DevFlow.Application.Abstractions;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Application.DomainEvents.ProjectCreated
{
    public class ProjectCreatedEventHandler
        : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

        public ProjectCreatedEventHandler(
            INotificationService notificationService,
            IWorkspaceMemberRepository workspaceMemberRepository)
        {
            _notificationService = notificationService;
            _workspaceMemberRepository = workspaceMemberRepository;
        }

        public async Task Handle(
            ProjectCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            var members = await _workspaceMemberRepository
                .GetAllByWorkspaceIdAsync(notification.WorkspaceId);

            foreach (var member in members)
            {
                if (member.UserId == notification.CreatedBy)
                    continue;

                await _notificationService.NotifyAsync(
                    member.UserId,
                    $"Project '{notification.ProjectName}' was created.",
                    NotificationType.ProjectCreated);
            }
        }
    }
}