using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Notifications
{
    public class NotificationsTests
    {
        [Fact]
        public void Should_Create_Notification_With_Valid_Data()
        {
            // Arrange
            var userId = 1;
            var message = "Task Assigned";
            var type = NotificationType.Workflow;
            var referenceId = 10;

            // Act
            var notification = new Notification(
                userId,
                message,
                type,
                referenceId);

            // Assert
            notification.UserId.Should().Be(userId);
            notification.Message.Should().Be(message);
            notification.Type.Should().Be(type);
            notification.ReferenceId.Should().Be(referenceId);
            notification.IsRead.Should().BeFalse();
        }
        [Fact]
        public void Should_Throw_When_UserId_Is_Invalid()
        {
            // Arrange
            var userId = 0;

            // Act
            Action act = () => new Notification(
                userId,
                "Task Assigned",
                NotificationType.Workflow,
                10);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid user id.*")
                .And.ParamName.Should().Be("userId");
        }
        [Fact]
        public void Should_Throw_When_Message_Is_Empty()
        {
            // Arrange
            var message = "";

            // Act
            Action act = () => new Notification(
                1,
                message,
                NotificationType.Workflow,
                10);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Message cannot be empty.*")
                .And.ParamName.Should().Be("message");
        }
        [Fact]
        public void Should_Throw_When_ReferenceId_Is_Invalid()
        {
            // Arrange
            var referenceId = 0;

            // Act
            Action act = () => new Notification(
                1,
                "Task Assigned",
                NotificationType.Workflow,
                referenceId);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("ReferenceId must be greater than zero.*")
                .And.ParamName.Should().Be("referenceId");
        }
        [Fact]
        public void Should_Mark_Notification_As_Read()
        {
            // Arrange
            var notification = new Notification(
                1,
                "Task Assigned",
                NotificationType.Workflow,
                10);

            // Act
            notification.MarkAsRead();

            // Assert
            notification.IsRead.Should().BeTrue();
        }
        [Fact]
        public void Should_Not_Mark_Notification_As_Read_When_Already_Read()
        {
            // Arrange
            var notification = new Notification(
                1,
                "Task Assigned",
                NotificationType.Workflow,
                10);

            notification.MarkAsRead();

            // Act
            notification.MarkAsRead();

            // Assert
            notification.IsRead.Should().BeTrue();
        }

    }
}
