using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.GetTaskById;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.GetTaskById
{
    public class GetTaskByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Task_By_Id()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                10,
                1,
                DateTime.UtcNow.AddDays(2),
                DevFlow.Domain.Enum.TaskPriority.High);


            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);


            var query = new GetTaskByIdQuery
            {
                TaskId = 10,
                ProjectId = 10
            };


            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);


            // Assert
            result.Should().NotBeNull();

            result.TaskId.Should().Be(task.Id);
            result.Title.Should().Be("Implement JWT");
            result.Description.Should().Be("Task Description");


            // Verify
            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);


            var query = new GetTaskByIdQuery
            {
                TaskId = 10,
                ProjectId = 10
            };


            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((TaskItem?)null);



            // Act
            Func<Task> act = () =>
                handler.Handle(query, CancellationToken.None);



            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();



            // Verify
            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);
        }



        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Belongs_To_Different_Project()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);



            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                20,
                1,
                DateTime.UtcNow.AddDays(2),
                DevFlow.Domain.Enum.TaskPriority.High);



            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);



            var query = new GetTaskByIdQuery
            {
                TaskId = 10,
                ProjectId = 10
            };



            // Act
            Func<Task> act = () =>
                handler.Handle(query, CancellationToken.None);



            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();


            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);
        }



        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Project_Member()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new GetTaskByIdHandler(
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);



            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                10,
                1,
                DateTime.UtcNow.AddDays(2),
                DevFlow.Domain.Enum.TaskPriority.High);



            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);



            projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(10))
                .ThrowsAsync(
                    new UnauthorizedException("Not project member"));



            var query = new GetTaskByIdQuery
            {
                TaskId = 10,
                ProjectId = 10
            };



            // Act
            Func<Task> act = () =>
                handler.Handle(query, CancellationToken.None);



            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizedException>();



            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);
        }
    }
}