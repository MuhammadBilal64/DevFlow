using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Notifications.GetNotifications;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Notifications.GetNotifications
{
    public class GetNotificationsHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Notifications()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();

            var handler = new GetNotificationsHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetNotificationsQuery
            {
                SearchTerm = "",
                IsRead = null,
                SortBy = "",
                Descending = false,
                PageNumber = 1,
                PageSize = 10
            };

            var notifications = new List<Notification>
            {
                new Notification(
                    1,
                    "Task Assigned",
                    NotificationType.TaskAssigned,
                    15),

                new Notification(
                    1,
                    "Task Completed",
                    NotificationType.TaskCompleted,
                    20)
            };

            var paginatedData = new PaginatedData<Notification>
            {
                Items = notifications,
                TotalCount = 2
            };

            notificationRepositoryMock
                .Setup(x => x.GetByUserIdAsync(
                    1,
                    "",
                    null,
                    "",
                    false,
                    1,
                    10))
                .ReturnsAsync(paginatedData);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.Items.Should().HaveCount(2);

            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);

            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();

            result.Items[0].Message.Should().Be("Task Assigned");
            result.Items[0].Type.Should().Be(NotificationType.TaskAssigned);

            result.Items[1].Message.Should().Be("Task Completed");
            result.Items[1].Type.Should().Be(NotificationType.TaskCompleted);

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetByUserIdAsync(
                    1,
                    "",
                    null,
                    "",
                    false,
                    1,
                    10),
                Times.Once);
        }
    }
}