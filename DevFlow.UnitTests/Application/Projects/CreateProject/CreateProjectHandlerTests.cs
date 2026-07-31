using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.CreateProject;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.CreateProject
{
    public class CreateProjectHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock;
        private readonly Mock<IWorkspaceAuthorizationService> _workspaceAuthorizationServiceMock;
        private readonly Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CreateProjectHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            _workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);
        }

        private CreateProjectHandler CreateHandler()
        {
            return new CreateProjectHandler(
                _projectMemberRepositoryMock.Object,
                _workspaceAuthorizationServiceMock.Object,
                _workspaceRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserServiceMock.Object,
                _projectRepositoryMock.Object);
        }


        [Fact]
        public async Task Should_Create_Project_Successfully()
        {
            var handler = CreateHandler();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };

            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };

            _workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);

            _projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(10, "AI Project"))
                .ReturnsAsync(false);


            var result = await handler.Handle(command, CancellationToken.None);


            result.Should().NotBeNull();
            result.ProjectName.Should().Be("AI Project");


            _workspaceRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            _workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(10),
                Times.Once);

            _projectRepositoryMock.Verify(
                x => x.ExistsInWorkspaceAsync(10, "AI Project"),
                Times.Once);

            _projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Once);

            _projectMemberRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ProjectMember>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workspace_Does_Not_Exist()
        {
            var handler = CreateHandler();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };


            _workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workspace?)null);


            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            await act.Should()
                .ThrowAsync<NotFoundException>();


            _workspaceAuthorizationServiceMock.Verify(
                x => x.EnsureAdminOrOwnerAsync(It.IsAny<int>()),
                Times.Never);

            _projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Never);

            _projectMemberRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ProjectMember>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Admin_Or_Owner()
        {
            var handler = CreateHandler();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };


            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };


            _workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);


            _workspaceAuthorizationServiceMock
                .Setup(x => x.EnsureAdminOrOwnerAsync(10))
                .ThrowsAsync(new UnauthorizedException("Not Authorized"));


            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            await act.Should()
                .ThrowAsync<UnauthorizedException>();


            _projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Never);

            _projectMemberRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ProjectMember>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Fact]
        public async Task Should_Throw_ConflictException_When_Project_Already_Exists()
        {
            var handler = CreateHandler();

            var command = new CreateProjectCommand
            {
                ProjectName = "AI Project",
                Description = "Project Description",
                WorkspaceId = 10
            };


            var workspace = new Workspace
            {
                Id = 10,
                Name = "Development",
                CreatedBy = 1
            };


            _workspaceRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workspace);


            _projectRepositoryMock
                .Setup(x => x.ExistsInWorkspaceAsync(10, "AI Project"))
                .ReturnsAsync(true);


            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            await act.Should()
                .ThrowAsync<ConflictException>();


            _projectRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Project>()),
                Times.Never);


            _projectMemberRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ProjectMember>()),
                Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}