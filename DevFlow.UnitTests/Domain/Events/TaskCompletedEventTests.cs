using DevFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Events
{
    public class TaskCompletedEventTests
    {
        [Fact]
        public void Should_Create_TaskCompletedEvent_With_Valid_Data()
        {
            // Arrange
            var recipientUserId = 10;
            var taskId = 5;
            var taskTitle = "Implement JWT";

            // Act
            var domainEvent = new TaskCompletedEvent(
                recipientUserId,
                taskId,
                taskTitle);

            // Assert
            domainEvent.RecipientUserId.Should().Be(recipientUserId);
            domainEvent.TaskId.Should().Be(taskId);
            domainEvent.TaskTitle.Should().Be(taskTitle);
        }

    }
}
