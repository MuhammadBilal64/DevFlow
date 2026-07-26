using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workspaces.GetWorkspaces;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workspaces.GetWorkspaces
{
    public class GetMyWorkspacesHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Workspaces()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var handler = new GetMyWorkspacesHandler(
                currentUserServiceMock.Object,
                workspaceMemberRepositoryMock.Object);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(1);

            var query = new GetMyWorkspacesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };

            var workspaceMembers = new List<WorkspaceMember>
        {
            new WorkspaceMember
            {
                Workspace = new Workspace
                {
                    Id = 1,
                    Name = "Development"
                }
            },
            new WorkspaceMember
            {
                Workspace = new Workspace
                {
                    Id = 2,
                    Name = "Marketing"
                }
            }
        };

            var paginatedData = new PaginatedData<WorkspaceMember>
            {
                Items = workspaceMembers,
                TotalCount = 2
            };

            workspaceMemberRepositoryMock
                .Setup(x => x.GetByUserIdAsync(1, "", "", false, 1, 10))
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

            result.Items[0].Id.Should().Be(1);
            result.Items[0].Name.Should().Be("Development");

            result.Items[1].Id.Should().Be(2);
            result.Items[1].Name.Should().Be("Marketing");

            workspaceMemberRepositoryMock.Verify(
                x => x.GetByUserIdAsync(1, "", "", false, 1, 10),
                Times.Once);
        }
    }
}
