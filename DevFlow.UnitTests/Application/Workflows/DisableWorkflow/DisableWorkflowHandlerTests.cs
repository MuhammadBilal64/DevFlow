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
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();

            var handler = new DisableWorkflowHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object);


            var project = new Project(
                "DevFlow",
                "Project Description",
                1,
                1);


            var workflow = new Workflow(
                project,
                1,
                "Approval Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);


            workflow.Enable();

            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(workflow);


            var command = new DisableWorkflowCommand
            {
                WorkflowId = 10,
                ProjectId = project.Id
            };


            // Act
            await handler.Handle(command, CancellationToken.None);


            // Assert
            workflow.IsEnabled.Should().BeFalse();


            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationServiceMock.Verify(
                x => x.EnsureCanManageProjectAsync(project.Id),
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
            var projectAuthorizationServiceMock = new Mock<IProjectAuthorizationService>();


            var handler = new DisableWorkflowHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationServiceMock.Object,
                unitOfWorkMock.Object);


            workflowRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((Workflow?)null);


            var command = new DisableWorkflowCommand
            {
                WorkflowId = 10,
                ProjectId = 1
            };


            // Act
            Func<Task> act = () =>
                handler.Handle(command, CancellationToken.None);


            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();


            workflowRepositoryMock.Verify(
                x => x.GetByIdAsync(10),
                Times.Once);


            projectAuthorizationServiceMock.Verify(
                x => x.EnsureCanManageProjectAsync(It.IsAny<int>()),
                Times.Never);


            workflowRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Workflow>()),
                Times.Never);


            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}