using DevFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Events
{
    public class ProjectCreatedEventTests
    {
        [Fact]
        public void Should_Create_ProjectCreatedEvent_With_Valid_Data()
        {
            // Arrange
            var projectName = "Project Alpha";
            var workspaceId = 1;
            var createdBy = 10;

            // Act
            var domainEvent = new ProjectCreatedEvent(
                projectName,
                workspaceId,
                createdBy);

            // Assert
            domainEvent.ProjectName.Should().Be(projectName);
            domainEvent.WorkspaceId.Should().Be(workspaceId);
            domainEvent.CreatedBy.Should().Be(createdBy);
        }
    }
}
