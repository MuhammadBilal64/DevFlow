using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
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
        public async Task Should_Create_Workflow_Successfully()
        {
            // Arrange

            var workflowRepositoryMock =
                new Mock<IWorkflowRepository>();

            var projectRepositoryMock =
                new Mock<IProjectRepository>();

            var projectAuthorizationMock =
                new Mock<IProjectAuthorizationService>();

            var unitOfWorkMock =
                new Mock<IUnitOfWork>();

            var currentUserServiceMock =
                new Mock<ICurrentUserService>();


            var handler = new CreateWorkflowHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object);



            var project = new Project(
                "DevFlow",
                "Project Management",
                1,
                1);
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 1);



            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(project);


            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(10);


            workflowRepositoryMock
                .Setup(x => x.ExistsInProjectAsync(
                    1,
                    "High Priority Notification"))
                .ReturnsAsync(false);



            var command = new CreateWorkflowCommand
            {
                ProjectId = 1,

                Name = "High Priority Notification",

                Description =
                    "Notify when high priority task is created",

                Trigger =
                    WorkflowTrigger.TaskAssigned,

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
                        ActionType =
                            WorkflowActionType.NotifyUser,

                        Parameters =
                            """
                            {
                                "recipient":"Assignee",
                                "message":"High priority task created"
                            }
                            """,

                        Order = 1
                    }
                }
            };


            // Act

            var result =
                await handler.Handle(
                    command,
                    CancellationToken.None);



            // Assert

            result.Should().NotBeNull();

            result.Name
                .Should()
                .Be(command.Name);

            result.Trigger
                .Should()
                .Be(command.Trigger);



            workflowRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Workflow>(w =>
                        w.Name == command.Name &&
                        w.Description == command.Description &&
                        w.Trigger == command.Trigger &&
                        w.ProjectId == 0 &&
                        w.Conditions.Count == 1 &&
                        w.Actions.Count == 1)),
                Times.Once);



            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);



            projectAuthorizationMock.Verify(
                x => x.EnsureCanManageProjectAsync(1),
                Times.Once);
        }



        [Fact]
        public async Task Should_Throw_NotFoundException_When_Project_Does_Not_Exist()
        {
            // Arrange

            var workflowRepositoryMock =
                new Mock<IWorkflowRepository>();

            var projectRepositoryMock =
                new Mock<IProjectRepository>();

            var projectAuthorizationMock =
                new Mock<IProjectAuthorizationService>();

            var unitOfWorkMock =
                new Mock<IUnitOfWork>();

            var currentUserServiceMock =
                new Mock<ICurrentUserService>();


            var handler = new CreateWorkflowHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object);



            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Project?)null);



            var command = new CreateWorkflowCommand
            {
                ProjectId = 1,
                Name = "Workflow",
                Trigger = WorkflowTrigger.TaskAssigned
            };



            // Act

            Func<Task> act = () =>
                handler.Handle(
                    command,
                    CancellationToken.None);



            // Assert

            await act.Should()
                .ThrowAsync<NotFoundException>();


            workflowRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Workflow>()),
                Times.Never);


            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }



        [Fact]
        public async Task Should_Throw_ConflictException_When_Workflow_Name_Already_Exists()
        {
            // Arrange

            var workflowRepositoryMock =
                new Mock<IWorkflowRepository>();

            var projectRepositoryMock =
                new Mock<IProjectRepository>();

            var projectAuthorizationMock =
                new Mock<IProjectAuthorizationService>();

            var unitOfWorkMock =
                new Mock<IUnitOfWork>();

            var currentUserServiceMock =
                new Mock<ICurrentUserService>();


            var handler = new CreateWorkflowHandler(
                workflowRepositoryMock.Object,
                projectAuthorizationMock.Object,
                projectRepositoryMock.Object,
                unitOfWorkMock.Object,
                currentUserServiceMock.Object);



            var project = new Project(
                "DevFlow",
                "Description",
                1,
                1);
            typeof(Project)
    .GetProperty(nameof(Project.Id))!
    .SetValue(project, 1);



            projectRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(project);


            workflowRepositoryMock
                .Setup(x => x.ExistsInProjectAsync(
                    1,
                    "Workflow"))
                .ReturnsAsync(true);



            var command = new CreateWorkflowCommand
            {
                ProjectId = 1,
                Name = "Workflow",
                Trigger = WorkflowTrigger.TaskAssigned
            };



            // Act

            Func<Task> act = () =>
                handler.Handle(
                    command,
                    CancellationToken.None);



            // Assert

            await act.Should()
                .ThrowAsync<ConflictException>();


            workflowRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Workflow>()),
                Times.Never);


            unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}