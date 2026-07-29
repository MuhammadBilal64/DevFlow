using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.UpdateWorkflow;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.UpdateWorkflow
{
    public class UpdateWorkflowHandlerTests
    {
        [Fact]
        public async Task Should_Update_Workflow()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            var workflow = new Workflow(
                "Old Workflow",
                "Old Description",
                WorkflowTrigger.TaskAssigned);

            workflow.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "Low"));

            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=1",
                    1));

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workflow);

            var command = new UpdateWorkflowCommand
            {
                WorkflowId = 10,
                Name = "Updated Workflow",
                Description = "Updated Description",

                Conditions =
                {
                    new WorkflowConditionDto
                    {
                        Field = "Status",
                        Operator = WorkflowOperator.Equals,
                        Value = "Completed"
                    }
                },

                Actions =
                {
                    new WorkflowActionDto
                    {
                        ActionType = WorkflowActionType.NotifyUser,
                        Parameters = "UserId=5",
                        Order = 1
                    }
                }
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            workflow.Name.Should().Be(command.Name);
            workflow.Description.Should().Be(command.Description);

            workflow.Conditions.Should().HaveCount(1);
            workflow.Actions.Should().HaveCount(1);

            workflow.Conditions.First().Field.Should().Be("Status");
            workflow.Conditions.First().Value.Should().Be("Completed");

            workflow.Actions.First().ActionType.Should().Be(WorkflowActionType.NotifyUser);
            workflow.Actions.First().Parameters.Should().Be("UserId=5");
            workflow.Actions.First().Order.Should().Be(1);

            // Verifies
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

            var handler = new UpdateWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);

            var command = new UpdateWorkflowCommand
            {
                WorkflowId = 10
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            // Verifies
            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Workflow>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}