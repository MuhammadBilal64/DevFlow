using DevFlow.Application.Abstractions;
using DevFlow.Application.DomainEvents.ProjectCreated;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using DevFlow.Domain.Events;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.DomainEvents.ProjectCreated
{
    public class ProjectCreatedEventHandlerTests
    {
        [Fact]
        public async Task Should_Notify_Workspace_Members_When_Project_Is_Created()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new ProjectCreatedEventHandler(
                notificationServiceMock.Object,
                workspaceMemberRepositoryMock.Object);


            var members = new List<WorkspaceMember>
            {
                new WorkspaceMember
                {
                    UserId = 2,
                    WorkspaceId = 1
                },
                new WorkspaceMember
                {
                    UserId = 3,
                    WorkspaceId = 1
                }
            };


            workspaceMemberRepositoryMock
                .Setup(x => x.GetAllByWorkspaceIdAsync(1))
                .ReturnsAsync(members);


            var domainEvent = new ProjectCreatedEvent(
                "DevFlow",
                1,
                1);


            // Act
            await handler.Handle(
                domainEvent,
                CancellationToken.None);


            // Assert
            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    2,
                    "Project 'DevFlow' was created.",
                    NotificationType.ProjectCreated),
                Times.Once);


            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    3,
                    "Project 'DevFlow' was created.",
                    NotificationType.ProjectCreated),
                Times.Once);
        }


        [Fact]
        public async Task Should_Not_Notify_Project_Creator()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new ProjectCreatedEventHandler(
                notificationServiceMock.Object,
                workspaceMemberRepositoryMock.Object);


            var members = new List<WorkspaceMember>
            {
                new WorkspaceMember
                {
                    UserId = 1,
                    WorkspaceId = 1
                }
            };


            workspaceMemberRepositoryMock
                .Setup(x => x.GetAllByWorkspaceIdAsync(1))
                .ReturnsAsync(members);


            var domainEvent = new ProjectCreatedEvent(
                "DevFlow",
                1,
                1);


            // Act
            await handler.Handle(
                domainEvent,
                CancellationToken.None);


            // Assert
            notificationServiceMock.Verify(
                x => x.NotifyAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<NotificationType>()),
                Times.Never);
        }
    }
}