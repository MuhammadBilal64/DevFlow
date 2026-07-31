using DevFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Events
{
    public class TaskAssignedEventTests
    {
        [Fact]
        public void Should_Create_TaskAssignedEvent_With_Valid_Data()
        {
            // Arrange
            var userId = 10;
            var taskId = 5;
            var projectId = 2;
            var taskTitle = "Implement JWT";

            // Act
            var domainEvent = new TaskAssignedEvent(
                userId,
                taskId,
                projectId,
                taskTitle);

            // Assert
            domainEvent.UserId.Should().Be(userId);
            domainEvent.TaskId.Should().Be(taskId);
            domainEvent.ProjectId.Should().Be(projectId);
            domainEvent.TaskTitle.Should().Be(taskTitle);
        }
    }
}