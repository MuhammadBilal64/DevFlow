using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Tasks.CreateTask;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.CreateTask
{
    public class CreateTaskHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IProjectAuthorizationService> _projectAuthorizationServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CreateTaskHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);
        }


        private CreateTaskHandler CreateHandler()
        {
            return new CreateTaskHandler(
                _projectAuthorizationServiceMock.Object,
                _currentUserServiceMock.Object,
                _projectRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _taskRepositoryMock.Object);
        }


        [Fact]
        public async Task Should_Create_Task_Successfully()
        {
            var handler = CreateHandler();

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5)
            };


            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);


            typeof(Project)
                .GetProperty(nameof(Project.Id))!
                .SetValue(project, 10);


            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);


            var result = await handler.Handle(command, CancellationToken.None);


            result.Should().NotBeNull();
            result.Title.Should().Be("Implement JWT");
            result.Description.Should().Be("Complete authentication module");


            _projectRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            _projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(10),
                Times.Once);


            _taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Once);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            var handler = CreateHandler();

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High
            };


            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Project?)null);


            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            await act.Should()
                .ThrowAsync<NotFoundException>();


            _projectAuthorizationServiceMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);


            _taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Project_Member()
        {
            var handler = CreateHandler();

            var command = new CreateTaskCommand
            {
                ProjectId = 10,
                Title = "Implement JWT",
                Description = "Complete authentication module",
                Priority = TaskPriority.High
            };


            var project = new Project(
                "DevFlow",
                "Project Description",
                5,
                1);


            typeof(Project)
                .GetProperty(nameof(Project.Id))!
                .SetValue(project, 10);


            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(project);


            _projectAuthorizationServiceMock
                .Setup(x => x.EnsureProjectMemberAsync(10))
                .ThrowsAsync(new UnauthorizedException("Not project member"));


            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            await act.Should()
                .ThrowAsync<UnauthorizedException>();


            _taskRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>()),
                Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}