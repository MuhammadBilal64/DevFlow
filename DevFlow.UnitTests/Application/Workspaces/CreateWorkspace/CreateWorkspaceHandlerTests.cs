using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Workspaces.CreateWorkspace;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.CreateWorkspace
{
    public class CreateWorkspaceHandlerTests
    {
        [Fact]
        public async Task Should_Create_Workspace_Successfully()
        {
           
            // Arrange
            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();

            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var currentUserServiceMock = new Mock<ICurrentUserService>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            currentUserServiceMock.Setup(x => x.UserId).Returns(1);
            var handler = new CreateWorkspaceHandler(
            unitOfWorkMock.Object,
         workspaceRepositoryMock.Object,
         currentUserServiceMock.Object,
         workspaceMemberRepositoryMock.Object);
            var command = new CreateWorkspaceCommand
            {
                Name = "Development"
            };
            //Act
            var result = await handler.Handle(command, CancellationToken.None);
            //assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Development");
            // Verify interactions

            workspaceRepositoryMock.Verify(
     x => x.AddAsync(It.Is<Workspace>(w =>
         w.Name == "Development" &&
         w.CreatedBy == 1)),
     Times.Once);

            workspaceMemberRepositoryMock.Verify(
    x => x.AddAsync(It.Is<WorkspaceMember>(m =>
        m.UserId == 1 &&
        m.Role == WorkspaceRole.Owner)),
    Times.Once);

            unitOfWorkMock.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);




        }
    }
}
