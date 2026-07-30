using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Projects.GetProjectsByWorkspace;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetProjectsByWorkspace
{
    public class GetProjectsByWorkspaceHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Projects_By_Workspace()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetProjectsByWorkspaceHandler(
                currentUserServiceMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = 10,
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };

            var member = new WorkspaceMember
            {
                UserId = 1,
                WorkspaceId = 10
            };

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(1, 10))
                .ReturnsAsync(member);

            var projects = new List<Project>
    {
        new Project(
            "AI Project",
            "AI Description",
            10,
            1),

        new Project(
            "CRM",
            "CRM Description",
            10,
            1)
    };

            var paginatedData = new PaginatedData<Project>
            {
                Items = projects,
                TotalCount = 2
            };

            projectRepositoryMock
                .Setup(x => x.GetProjectsByWorkspaceAsync(10, "", "", false, 1, 10))
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

            result.Items[0].ProjectName.Should().Be("AI Project");
            result.Items[0].Description.Should().Be("AI Description");

            result.Items[1].ProjectName.Should().Be("CRM");
            result.Items[1].Description.Should().Be("CRM Description");

            // Verifies
            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(1, 10),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.GetProjectsByWorkspaceAsync(10, "", "", false, 1, 10),
                Times.Once);
        }
        [Fact]
        public async Task Should_Throw_UnauthorizedException_When_User_Is_Not_A_Workspace_Member()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetProjectsByWorkspaceHandler(
                currentUserServiceMock.Object,
                projectRepositoryMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetMyProjectsByWorkspaceQuery
            {
                WorkspaceId = 10,
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };

            workspaceMemberRepositoryMock
                .Setup(x => x.GetMemberAsync(1, 10))
                .ReturnsAsync((WorkspaceMember?)null);

            // Act
            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();

            // Verifies
            workspaceMemberRepositoryMock.Verify(
                x => x.GetMemberAsync(1, 10),
                Times.Once);

            projectRepositoryMock.Verify(
                x => x.GetProjectsByWorkspaceAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        } 

    }
}
