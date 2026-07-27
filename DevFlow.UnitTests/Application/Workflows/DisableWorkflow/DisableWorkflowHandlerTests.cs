using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.DisableWorkflow;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.DisableWorkflow
{
    public class DisableWorkflowHandlerTests
    {
        [Fact]
        public async Task Should_Disable_Workflow()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new DisableWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            var workflow = new Workflow(
                "Approval Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workflow);

            var command = new DisableWorkflowCommand
            {
                WorkflowId = 10
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            workflow.IsEnabled.Should().BeFalse();

            // Verifies
            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(workflow),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workflow_Does_Not_Exist()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new DisableWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);

            var command = new DisableWorkflowCommand
            {
                WorkflowId = 10
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);

            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Workflow>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}