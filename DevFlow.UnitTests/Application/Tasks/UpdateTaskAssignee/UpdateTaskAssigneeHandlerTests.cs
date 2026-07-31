using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.UpdateTaskAssignee;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.UpdateTaskAssignee
{
    public class UpdateTaskAssigneeHandlerTests
    {
        [Fact]
        public async Task Should_Assign_Task_Successfully()
        {
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                ProjectId = 1,
                NewAssigneeId = 5
            };

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                1,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(1),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(1, 5),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(task),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Fact]
        public async Task Should_Unassign_Task_Successfully()
        {
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                ProjectId = 1,
                NewAssigneeId = null
            };

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                1,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            task.Assign(5);

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(1),
                Times.Once);

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(task),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                ProjectId = 1,
                NewAssigneeId = 5
            };

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((TaskItem?)null);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            taskRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_New_Assignee_Is_Not_Project_Member()
        {
            var taskRepositoryMock = new Mock<ITaskRepository>();
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateTaskAssigneeHandler(
                unitOfWorkMock.Object,
                taskRepositoryMock.Object,
                projectAuthorizationServiceMock.Object);

            var command = new UpdateTaskAssigneeCommand
            {
                TaskId = 10,
                ProjectId = 1,
                NewAssigneeId = 5
            };

            var task = new TaskItem(
                "Implement JWT",
                "Task Description",
                1,
                1,
                DateTime.UtcNow.AddDays(2),
                TaskPriority.High);

            taskRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(task);

            projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(1, 5))
                .ThrowsAsync(new UnauthorizedException("Not project member"));

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>();

            projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(1, 5),
                Times.Once);

            taskRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}