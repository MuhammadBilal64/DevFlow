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
            var authorizationMock = new Mock<IProjectAuthorizationService>();

            var handler = new UpdateWorkflowHandler(
                workflowRepositoryMock.Object,
                authorizationMock.Object,
                unitOfWorkMock.Object);

            var project = new Project(
                "Project",
                "Description",
                1,
                1);

            // Set Project.Id via reflection
            typeof(Project)
                .GetProperty(nameof(Project.Id))!
                .SetValue(project, 1);

            var workflow = new Workflow(
                project,
                1,
                "Old Workflow",
                "Old Description",
                WorkflowTrigger.TaskAssigned);

            typeof(Workflow)
    .GetProperty(nameof(Workflow.ProjectId))!
    .SetValue(workflow, 1);

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

            authorizationMock
                .Setup(x => x.EnsureCanManageProjectAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            workflowRepositoryMock
                .Setup(x => x.ExistsInProjectAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>()))
                .ReturnsAsync(false);

            var command = new UpdateWorkflowCommand
            {
                WorkflowId = 10,
                ProjectId = 1,
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
            workflow.Name.Should().Be("Updated Workflow");
            workflow.Description.Should().Be("Updated Description");

            workflow.Conditions.Should().HaveCount(1);
            workflow.Actions.Should().HaveCount(1);

            workflow.Conditions.First().Field.Should().Be("Status");
            workflow.Actions.First().Parameters.Should().Be("UserId=5");

            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(workflow),
                Times.Once);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            authorizationMock.Verify(
                x => x.EnsureCanManageProjectAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workflow_Does_Not_Exist()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var authorizationMock = new Mock<IProjectAuthorizationService>();

            var handler = new UpdateWorkflowHandler(
                workflowRepositoryMock.Object,
                authorizationMock.Object,
                unitOfWorkMock.Object);

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);

            var command = new UpdateWorkflowCommand
            {
                WorkflowId = 10,
                ProjectId = 1
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Workflow>()),
                Times.Never);

            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}