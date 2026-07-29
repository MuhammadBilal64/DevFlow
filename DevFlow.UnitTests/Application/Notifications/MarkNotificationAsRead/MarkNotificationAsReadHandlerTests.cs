using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Notifications.MarkNotificationAsRead;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Notifications.MarkNotificationAsRead
{
    public class MarkNotificationAsReadHandlerTests
    {
        [Fact]
        public async Task Should_Mark_Notification_As_Read()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new MarkNotificationAsReadHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object,
                unitOfWorkMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var notification = new Notification(
                1,
                "Task Assigned",
                NotificationType.TaskAssigned,
                10);

            notificationRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(notification);

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = 10
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Message.Should().Be("Notification marked as read.");
            notification.IsRead.Should().BeTrue();

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
        [Fact]
        public async Task Should_Throw_NotFoundException_When_Notification_Does_Not_Exist()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new MarkNotificationAsReadHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object,
                unitOfWorkMock.Object);

            notificationRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Notification?)null);

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = 10
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
        [Fact]
        public async Task Should_Throw_ForbiddenException_When_Notification_Does_Not_Belong_To_User()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var notificationRepositoryMock = new Mock<INotificationRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new MarkNotificationAsReadHandler(
                currentUserServiceMock.Object,
                notificationRepositoryMock.Object,
                unitOfWorkMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var notification = new Notification(
                2, // Different user
                "Task Assigned",
                NotificationType.TaskAssigned,
                10);

            notificationRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(notification);

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = 10
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();

            // Verifies
            notificationRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}
