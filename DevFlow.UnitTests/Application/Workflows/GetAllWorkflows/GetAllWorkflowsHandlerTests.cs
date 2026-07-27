using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.GetAllWorkflows;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.GetAllWorkflows
{
    public class GetAllWorkflowHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paginated_Workflows()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();

            var handler = new GetAllWorkflowHandler(
                workflowRepositoryMock.Object);

            var query = new GetAllWorkflowsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "",
                SortBy = "",
                Descending = false
            };

            var workflow1 = new Workflow(
                "Approval Workflow",
                "Approval Description",
                WorkflowTrigger.TaskAssigned);

            var workflow2 = new Workflow(
                "Completion Workflow",
                "Completion Description",
                WorkflowTrigger.TaskCompleted);

            workflow2.Disable();

            var paginatedData = new PaginatedData<Workflow>
            {
                Items = new List<Workflow>
                {
                    workflow1,
                    workflow2
                },
                TotalCount = 2
            };

            workflowRepositoryMock
                .Setup(x => x.GetAllAsync(
                    "", null, null, "", false, 1, 10))
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

            result.Items[0].Name.Should().Be(workflow1.Name);
            result.Items[0].Description.Should().Be(workflow1.Description);
            result.Items[0].Trigger.Should().Be(workflow1.Trigger);
            result.Items[0].IsEnabled.Should().BeTrue();

            result.Items[1].Name.Should().Be(workflow2.Name);
            result.Items[1].Description.Should().Be(workflow2.Description);
            result.Items[1].Trigger.Should().Be(workflow2.Trigger);
            result.Items[1].IsEnabled.Should().BeFalse();

            // Verifies
            workflowRepositoryMock.Verify(
                x => x.GetAllAsync(
                    "", null, null, "", false, 1, 10),
                Times.Once);
        }
    }
}