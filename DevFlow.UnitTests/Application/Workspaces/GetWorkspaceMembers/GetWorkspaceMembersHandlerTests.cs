using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workspaces.GetWorkspaceMembers;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.GetWorkspaceMembers
{
    public class GetWorkspaceMembersHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Workspace_Members()
        {
            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new GetWorkspaceMembersHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
    .Setup(x => x.UserId)
    .Returns(1);
            var query = new GetWorkspaceMembersQuery
            {
                WorkspaceId = 10,
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };
            var members = new List<WorkspaceMember>
{
    new WorkspaceMember
    {
        UserId = 1,
        Role = WorkspaceRole.Owner,
        JoinedAt = DateTime.UtcNow,
        User = new User
        {
            Name = "Bilal"
        }
    },
    new WorkspaceMember
    {
        UserId = 2,
        Role = WorkspaceRole.Member,
        JoinedAt = DateTime.UtcNow,
        User = new User
        {
            Name = "Ali"
        }
    }
};
            var paginatedData = new PaginatedData<WorkspaceMember>
            {
                Items= members,
                TotalCount= 2,
            };
            workspaceMemberRepositoryMock
                .Setup(x => x.GetAllMembersAsync(10, "", "", false, 1, 10))
                .ReturnsAsync(paginatedData);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();

            result.Items.Should().HaveCount(2);

            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);

            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();

            result.Items[0].UserId.Should().Be(1);
            result.Items[0].Name.Should().Be("Bilal");
            result.Items[0].Role.Should().Be(WorkspaceRole.Owner);

            result.Items[1].UserId.Should().Be(2);
            result.Items[1].Name.Should().Be("Ali");
            result.Items[1].Role.Should().Be(WorkspaceRole.Member);

            //verifies
            workspaceAuthorizationServiceMock.Verify(x => x.EnsureAdminOrOwnerAsync(10), Times.Once);
            workspaceMemberRepositoryMock.Verify(x => x.GetAllMembersAsync(10, "", "", false, 1, 10), Times.Once);


        }
        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_Admin_Or_Owner()
        {

            // Arrange
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceAuthorizationServiceMock = new Mock<IWorkspaceAuthorizationService>();

            var handler = new GetWorkspaceMembersHandler(
                workspaceAuthorizationServiceMock.Object,
                workspaceMemberRepositoryMock.Object,
                currentUserServiceMock.Object);

            currentUserServiceMock
    .Setup(x => x.UserId)
    .Returns(1);
            var query = new GetWorkspaceMembersQuery
            {
                WorkspaceId = 10,
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };
            workspaceAuthorizationServiceMock.Setup(x => x.EnsureAdminOrOwnerAsync(10)).ThrowsAsync(new UnauthorizedException("Not Authorized"));


            Func<Task> act = () => handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizedException>();
            workspaceMemberRepositoryMock.Verify(x => x.GetAllMembersAsync(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            workspaceAuthorizationServiceMock.Verify(
    x => x.EnsureAdminOrOwnerAsync(10),
    Times.Once);
        }

    }
}
