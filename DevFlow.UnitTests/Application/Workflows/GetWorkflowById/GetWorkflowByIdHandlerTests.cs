using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Workflows.GetWorkflowById;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.GetWorkflowById
{
    public class GetWorkflowByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Workflow_By_Id()
        {
            // Arrange
            var workflowRepositoryMock =
                new Mock<IWorkflowRepository>();

            var projectAuthorizationMock =
                new Mock<IProjectAuthorizationService>();

            var handler = new GetWorkflowByIdHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationMock.Object);


            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                10);


            var workflow = new Workflow(
                project,
                10,
                "Approval Workflow",
                "Workflow Description",
                WorkflowTrigger.TaskAssigned);


            workflow.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));


            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=1",
                    2));


            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=2",
                    1));


            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workflow);


            var query = new GetWorkflowByIdQuery
            {
                ProjectId = project.Id,
                WorkflowId = 10
            };


            // Act
            var result =
                await handler.Handle(
                    query,
                    CancellationToken.None);


            // Assert

            result.Should().NotBeNull();

            result.Name.Should()
                .Be(workflow.Name);

            result.Description.Should()
                .Be(workflow.Description);

            result.Trigger.Should()
                .Be(workflow.Trigger);

            result.IsEnabled.Should()
                .BeFalse();


            result.Conditions.Should()
                .HaveCount(1);

            result.Conditions[0].Field
                .Should().Be("Priority");

            result.Conditions[0].Operator
                .Should().Be(WorkflowOperator.Equals);

            result.Conditions[0].Value
                .Should().Be("High");


            result.Actions.Should()
                .HaveCount(2);


            // Because handler sorts by Order
            result.Actions[0].Order
                .Should().Be(1);

            result.Actions[1].Order
                .Should().Be(2);


            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationMock.Verify(
                x => x.EnsureProjectMemberAsync(project.Id),
                Times.Once);
        }



        [Fact]
        public async Task Should_Throw_NotFoundException_When_Workflow_Does_Not_Exist()
        {
            // Arrange

            var workflowRepositoryMock =
                new Mock<IWorkflowRepository>();

            var projectAuthorizationMock =
                new Mock<IProjectAuthorizationService>();


            var handler = new GetWorkflowByIdHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationMock.Object);



            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);



            var query = new GetWorkflowByIdQuery
            {
                ProjectId = 1,
                WorkflowId = 10
            };


            // Act

            Func<Task> act = () =>
                handler.Handle(
                    query,
                    CancellationToken.None);



            // Assert

            await act.Should()
                .ThrowAsync<NotFoundException>();


            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationMock.Verify(
                x => x.EnsureProjectMemberAsync(It.IsAny<int>()),
                Times.Never);
        }
    }
}