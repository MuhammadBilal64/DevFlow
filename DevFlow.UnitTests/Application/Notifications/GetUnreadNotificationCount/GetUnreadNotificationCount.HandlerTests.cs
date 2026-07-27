using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Notifications.GetUnreadNotificationCount;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Notifications.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountHandlerTests
    {
        [Fact]
        public async Task Should_Return_Unread_Notification_Count()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();

            var handler = new GetUnreadNotificationCountHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            notificationRepositoryMock
                .Setup(x => x.GetUnreadCountAsync(1))
                .ReturnsAsync(5);

            var query = new GetUnreadNotificationCountQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UnreadCount.Should().Be(5);

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetUnreadCountAsync(1),
                Times.Once);
        }
    }
}