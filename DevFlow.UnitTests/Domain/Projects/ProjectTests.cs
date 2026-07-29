using DevFlow.Domain.Entities;
using DevFlow.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Projects
{
    public class ProjectTests
    {
        [Fact]
        public void Should_Create_Project_With_Valid_Data()
        {
            // Arrange
            var name = "DevFlow";
            var description = "Project Description";
            var workspaceId = 1;
            var createdBy = 2;

            // Act
            var project = new Project(
                name,
                description,
                workspaceId,
                createdBy);
            // Assert
            project.Name.Should().Be(name);
            project.Description.Should().Be(description);
            project.WorkspaceId.Should().Be(workspaceId);
            project.CreatedBy.Should().Be(createdBy);

            project.CreatedAt.Should()
    .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

            project.DomainEvents.Should().HaveCount(1);
            project.DomainEvents
    .Should()
    .ContainSingle(x => x is ProjectCreatedEvent);

        }

        [Fact]
        public void Should_Throw_When_Name_Is_Empty()
        {
            // Arrange
            var name = "";
            var description = "Project Description";
            var workspaceId = 1;
            var createdBy = 2;
            //Act

            Action act = () => new Project(name, description, workspaceId, createdBy);
            //Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Project name is required.").And.ParamName.Should().Be("Name");





        }

        [Fact]
        public void Should_Throw_When_Description_Is_Empty()
        {
            // Arrange
            var name = "DevFlow";
            var description = "";
            var workspaceId = 1;
            var createdBy = 2;

            // Act
            Action act = () => new Project(
                name,
                description,
                workspaceId,
                createdBy);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Project description is required.*")
                .And.ParamName.Should().Be("description");
        }
        [Fact]
        public void Should_Throw_When_WorkspaceId_Is_Invalid()
        {
            // Arrange
            var name = "DevFlow";
            var description = "Project Description";
            var workspaceId = 0;
            var createdBy = 2;

            // Act
            Action act = () => new Project(
                name,
                description,
                workspaceId,
                createdBy);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid workspace id.*")
                .And.ParamName.Should().Be("workspaceId");
        }

        [Fact]
        public void Should_Throw_When_CreatedBy_Is_Invalid()
        {
            // Arrange
            var name = "DevFlow";
            var description = "Project Description";
            var workspaceId = 1;
            var createdBy = 0;

            // Act
            Action act = () => new Project(
                name,
                description,
                workspaceId,
                createdBy);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Invalid creator id.*")
                .And.ParamName.Should().Be("createdBy");
        }


        // -----------------------------
        // UpdateName Tests
        // -----------------------------

        [Fact]
        public void Should_Update_Name()
        {
            // Arrange
            var project = new Project(
                "Old Name",
                "Project Description",
                1,
                2);

            // Act
            project.UpdateName("New Name");

            // Assert
            project.Name.Should().Be("New Name");
        }
        [Fact]
        public void Should_Not_Update_When_Name_Is_Same()
        {
            // Arrange
            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                2);

            // Act
            project.UpdateName("DevFlow");

            // Assert
            project.Name.Should().Be("DevFlow");
        }
        [Fact]
        public void Should_Throw_When_Updating_Name_To_Empty()
        {
            // Arrange
            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                2);

            // Act
            Action act = () => project.UpdateName("");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Project name is required.*")
                .And.ParamName.Should().Be("name");
        }


        // -----------------------------
        // UpdateDescription Tests
        // -----------------------------

        [Fact]
        public void Should_Update_Description()
        {
            // Arrange
            var project = new Project(
                "DevFlow",
                "Old Description",
                1,
                2);

            // Act
            project.UpdateDescription("New Description");

            // Assert
            project.Description.Should().Be("New Description");
        }


        [Fact]
        public void Should_Not_Update_When_Description_Is_Same()
        {
            // Arrange
            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                2);

            // Act
            project.UpdateDescription("Project Description");

            // Assert
            project.Description.Should().Be("Project Description");
        }
        [Fact]
        public void Should_Throw_When_Updating_Description_To_Empty()
        {
            // Arrange
            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                2);

            // Act
            Action act = () => project.UpdateDescription("");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Project description is required.*")
                .And.ParamName.Should().Be("description");
        }
    }
}
