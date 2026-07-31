using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.DeleteTask;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.DeleteTask
{
    public class DeleteTaskHandlerTests
    {
        [Fact]
        public async Task Should_Delete_Task_Successfully()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new DeleteTaskHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new DeleteTaskCommand
            {
                TaskId = 10,
                ProjectId = 1
            };
            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                1, // ProjectId must match command.ProjectId
                1,
                DateTime.UtcNow.AddDays(2),
                DevFlow.Domain.Enum.TaskPriority.High);

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TaskId.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(1),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.DeleteAsync(task),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            // Arrange
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new DeleteTaskHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new DeleteTaskCommand
            {
                TaskId = 10,
                ProjectId = 1
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);

            taskRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}