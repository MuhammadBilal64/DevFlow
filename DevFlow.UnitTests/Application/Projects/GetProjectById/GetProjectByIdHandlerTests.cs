using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.GetProjectById;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetProjectById
{
    public class GetProjectByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Project_By_Id()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            var project = new Project(
                "AI Project",
                "Project Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(10))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.ProjectId.Should().Be(project.Id);
            result.ProjectName.Should().Be("AI Project");
            result.Description.Should().Be("Project Description");
            result.CreatedAt.Should().Be(project.CreatedAt);

            // Verify
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();

            // Verify
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Project_Member()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new GetProjectByIdHandler(
                projectRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var query = new GetProjectByIdQuery
            {
                Id = 10
            };

            var project = new Project(
                "AI Project",
                "Project Description",
                5,
                1);

            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);
            projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(10))
                .ThrowsAsync(new UnauthorizedException("User is not authorized."));

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedException>();

            // Verify
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);
        }
    }
}