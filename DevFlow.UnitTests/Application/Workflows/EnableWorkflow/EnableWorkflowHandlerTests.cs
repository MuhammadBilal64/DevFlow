using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.EnableWorkflow;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.EnableWorkflow
{
    public class EnableWorkflowHandlerTests
    {
        [Fact]
        public async Task Should_Enable_Workflow()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new EnableWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            var workflow = new Workflow(
                "Approval Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflow.Disable();

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workflow);

            var command = new EnableWorkflowCommand
            {
                WorkflowId = 10
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            workflow.IsEnabled.Should().BeTrue();

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

            var handler = new EnableWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);

            var command = new EnableWorkflowCommand
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