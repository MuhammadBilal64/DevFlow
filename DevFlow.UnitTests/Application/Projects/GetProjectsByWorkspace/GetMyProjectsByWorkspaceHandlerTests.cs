using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Projects.GetMyProjectsByWorkspace;
using DevFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Projects.GetMyProjectsByWorkspace
{
    public class GetMyProjectsByWorkspaceHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Projects_For_User_In_Workspace()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();

            var handler = new GetMyProjectsByWorkspaceHandler(
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

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
                .Setup(x => x.GetProjectsForUserAsync(
                    10,
                    1,
                    "",
                    "",
                    false,
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

            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();

            result.Items[0].ProjectName.Should().Be("AI Project");
            result.Items[0].Description.Should().Be("AI Description");

            result.Items[1].ProjectName.Should().Be("CRM");
            result.Items[1].Description.Should().Be("CRM Description");


            projectRepositoryMock.Verify(
                x => x.GetProjectsForUserAsync(
                    10,
                    1,
                    "",
                    "",
                    false,
                    1,
                    10),
                Times.Once);
        }


        [Fact]
        public async Task Should_Return_Empty_Result_When_User_Has_No_Project_Membership()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            var projectRepositoryMock = new Mock<IProjectRepository>();

            var handler = new GetMyProjectsByWorkspaceHandler(
                currentUserServiceMock.Object,
                projectRepositoryMock.Object);

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


            projectRepositoryMock
                .Setup(x => x.GetProjectsForUserAsync(
                    10,
                    1,
                    "",
                    "",
                    false,
                    1,
                    10))
                .ReturnsAsync(new PaginatedData<Project>
                {
                    Items = new List<Project>(),
                    TotalCount = 0
                });


            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);


            // Assert
            result.Should().NotBeNull();

            result.Items.Should().BeEmpty();

            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);


            projectRepositoryMock.Verify(
                x => x.GetProjectsForUserAsync(
                    10,
                    1,
                    "",
                    "",
                    false,
                    1,
                    10),
                Times.Once);
        }
    }
}