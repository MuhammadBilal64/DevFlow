using DevFlow.Application.Abstractions;
using DevFlow.Application.Workflows.CreateWorkflow;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.CreateWorkflow
{
    public class CreateWorkflowHandlerTests
    {
        [Fact]
        public async Task Should_Create_Workflow()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new CreateWorkflowHandler(
                workflowRepositoryMock.Object,
                unitOfWorkMock.Object);

            var command = new CreateWorkflowCommand
            {
                Name = "High Priority Notification",
                Description = "Notify when high priority task is created",
                Trigger = WorkflowTrigger.TaskAssigned,
                Conditions =
                {
                    new WorkflowConditionDto
                    {
                        Field = "Priority",
                        Operator = WorkflowOperator.Equals,
                        Value = "High"
                    }
                },
                Actions =
                {
                    new WorkflowActionDto
                    {
                        ActionType = WorkflowActionType.NotifyUser,
                        Parameters = "UserId=1",
                        Order = 1
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(command.Name);
            result.Trigger.Should().Be(command.Trigger);

            workflowRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Workflow>(w =>
                    w.Name == command.Name &&
                    w.Description == command.Description &&
                    w.Trigger == command.Trigger &&
                    w.Conditions.Count == 1 &&
                    w.Actions.Count == 1)),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}