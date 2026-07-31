using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.GetTasksByProject;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.GetTasksByProject
{
    public class GetTasksByProjectHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Tasks_By_Project()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new GetTasksByProjectHandler(
                projectRepositoryMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);



            var query = new GetTasksByProjectQuery
            {
                ProjectId = 10,
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };



            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);


            typeof(Project)
                .GetProperty(nameof(Project.Id))!
                .SetValue(project, 10);



            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);



            var tasks = new List<TaskItem>
            {
                new TaskItem(
                    "Implement JWT",
                    "Task Description",
                    10,
                    1,
                    DateTime.UtcNow.AddDays(2),
                    DevFlow.Domain.Enum.TaskPriority.High),

                new TaskItem(
                    "Implement SignalR",
                    "SignalR Description",
                    10,
                    1,
                    DateTime.UtcNow.AddDays(3),
                    DevFlow.Domain.Enum.TaskPriority.Medium)
            };



            var paginatedData = new PaginatedData<TaskItem>
            {
                Items = tasks,
                TotalCount = 2
            };



            taskRepositoryMock
                .Setup(x => x.GetTasksByProjectAsync(
                    10,
                    "",
                    "",
                    false,
                    null,
                    null,
                    1,
                    10))
                .ReturnsAsync(paginatedData);



            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);



            // Assert
            result.Should().NotBeNull();

            result.Items.Should().HaveCount(2);

            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);

            result.Items[0].Title.Should().Be("Implement JWT");
            result.Items[1].Title.Should().Be("Implement SignalR");



            // Verify
            projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);



            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);



            taskRepositoryMock.Verify(
                x => x.GetTasksByProjectAsync(
                    10,
                    "",
                    "",
                    false,
                    null,
                    null,
                    1,
                    10),
                Times.Once);
        }



        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new GetTasksByProjectHandler(
                projectRepositoryMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);



            var query = new GetTasksByProjectQuery
            {
                ProjectId = 10,
                PageNumber = 1,
                PageSize = 10
            };



            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);



            // Act
            Func<Task> act = () =>
                handler.Handle(query, CancellationToken.None);



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



            taskRepositoryMock.Verify(
                x => x.GetTasksByProjectAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<DevFlow.Domain.Enum.TaskStatus?>(),
                    It.IsAny<DevFlow.Domain.Enum.TaskPriority?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        }
    }
}