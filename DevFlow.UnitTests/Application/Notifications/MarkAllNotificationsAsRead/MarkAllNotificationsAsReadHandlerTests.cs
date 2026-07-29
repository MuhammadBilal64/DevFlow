using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Notifications.MarkAllNotificationsAsRead;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Notifications.MarkAllNotificationsAsRead
{
    public class MarkAllNotificationsAsReadHandlerTests
    {
        [Fact]
        public async Task Should_Mark_All_Notifications_As_Read()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new MarkAllNotificationsAsReadHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object,
                unitOfWorkMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var notifications = new List<Notification>
            {
                new Notification(
                    1,
                    "Task Assigned",
                    NotificationType.TaskAssigned,
                    10),

                new Notification(
                    1,
                    "Task Completed",
                    NotificationType.TaskCompleted,
                    20)
            };

            notificationRepositoryMock
                .Setup(x => x.GetUnreadByUserIdAsync(1))
                .ReturnsAsync(notifications);

            var command = new MarkAllNotificationsAsReadCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("All notifications marked as read.");

            notifications.Should().OnlyContain(n => n.IsRead);

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetUnreadByUserIdAsync(1),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}