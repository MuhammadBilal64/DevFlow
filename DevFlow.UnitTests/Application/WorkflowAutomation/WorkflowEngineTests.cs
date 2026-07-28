using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows;
using DevFlow.Application.Workflows.ActionExecution;
using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using Moq;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation
{
    public class WorkflowEngineTests
    {
        [Fact]
        public async Task Should_Execute_All_Actions_When_All_Conditions_Pass()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);

            var workflow = new Workflow(
                "Assignment Workflow",
                "Notify on assignment",
                WorkflowTrigger.TaskAssigned);

            workflow.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "dummy",
                    1));

            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "dd",
                    2));

            workflowRepositoryMock
                .Setup(x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow> { workflow });

            conditionEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(true);

            actionDispatcherMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(Task.CompletedTask);

            var context = new WorkflowExecutionContext(
                new Dictionary<string, object?>());


            // Act
            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                context);

            // Assert
            workflowRepositoryMock.Verify(
                x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned),
                Times.Once);

            conditionEvaluatorMock.Verify(
                x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Once);

            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Should_Not_Execute_Actions_When_A_Condition_Fails()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);

            var workflow = new Workflow(
                "Assignment Workflow",
                "Notify on assignment",
                WorkflowTrigger.TaskAssigned);

            workflow.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "dummy",
                    1));

            workflow.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "dd",
                    2));

            workflowRepositoryMock
                .Setup(x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow> { workflow });

            conditionEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(false);
            var context = new WorkflowExecutionContext(
    new Dictionary<string, object?>());
            //Act

            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                context);
            //Assert 
            workflowRepositoryMock.Verify(
    x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned),
    Times.Once);

            conditionEvaluatorMock.Verify(
                x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Once);

            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);

        }

        [Fact]
        public async Task Should_Do_Nothing_When_No_Workflows_Are_Found()
        {
            // Arrange
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);

            workflowRepositoryMock
                .Setup(x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow>());

            var context = new WorkflowExecutionContext(
                new Dictionary<string, object?>());

            // Act
            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                context);

            // Assert
            workflowRepositoryMock.Verify(
                x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned),
                Times.Once);

            conditionEvaluatorMock.Verify(
                x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);

            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_Continue_With_Next_Workflow_When_Action_Throws_Exception()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);

            var workflow1 = new Workflow(
    "Workflow 1",
    "",
    WorkflowTrigger.TaskAssigned);

            workflow1.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow1.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=1",
                    1));
            var workflow2 = new Workflow(
    "Workflow 2",
    "",
    WorkflowTrigger.TaskAssigned);

            workflow2.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow2.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=2",
                    1));
            workflowRepositoryMock
    .Setup(x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned))
    .ReturnsAsync(new List<Workflow>
    {
        workflow1,
        workflow2
    });
            // Both conditions pass
            conditionEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
            It.IsAny<WorkflowExecutionContext>()))
        .Returns(true);
            // First action throws, second succeeds
            actionDispatcherMock
                .SetupSequence(x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .ThrowsAsync(new Exception("Dispatcher failed"))
                .Returns(Task.CompletedTask);

            var context = new WorkflowExecutionContext(
    new Dictionary<string, object?>());

            // Act
            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                context);

            // Assert
            workflowRepositoryMock.Verify(
                x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned),
                Times.Once);

            conditionEvaluatorMock.Verify(
                x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Exactly(2));

            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Exactly(2));



        }



        [Fact]
        public async Task Should_Continue_With_Next_Workflow_When_Condition_Evaluation_Throws_Exception()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);

            var workflow1 = new Workflow(
    "Workflow 1",
    "",
    WorkflowTrigger.TaskAssigned);

            workflow1.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow1.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=1",
                    1));
            var workflow2 = new Workflow(
    "Workflow 2",
    "",
    WorkflowTrigger.TaskAssigned);

            workflow2.AddCondition(
                new WorkflowCondition(
                    "Priority",
                    WorkflowOperator.Equals,
                    "High"));

            workflow2.AddAction(
                new WorkflowAction(
                    WorkflowActionType.NotifyUser,
                    "UserId=2",
                    1));
            workflowRepositoryMock
    .Setup(x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned))
    .ReturnsAsync(new List<Workflow>
    {
        workflow1,
        workflow2
    });

            conditionEvaluatorMock
    .SetupSequence(x => x.Evaluate(
        It.IsAny<WorkflowCondition>(),
        It.IsAny<WorkflowExecutionContext>()))
    .Throws(new Exception("Condition failed"))
    .Returns(true);
            actionDispatcherMock
    .Setup(x => x.ExecuteAsync(
        It.IsAny<WorkflowAction>(),
        It.IsAny<WorkflowExecutionContext>()))
    .Returns(Task.CompletedTask);
            var context = new WorkflowExecutionContext(
    new Dictionary<string, object?>());
            await engine.ExecuteAsync(
    WorkflowTrigger.TaskAssigned,
    context);
            workflowRepositoryMock.Verify(
    x => x.GetActiveByTriggerAsync(WorkflowTrigger.TaskAssigned),
    Times.Once);
            conditionEvaluatorMock.Verify(
    x => x.Evaluate(
        It.IsAny<WorkflowCondition>(),
        It.IsAny<WorkflowExecutionContext>()),
    Times.Exactly(2));

            actionDispatcherMock.Verify(
    x => x.ExecuteAsync(
        It.IsAny<WorkflowAction>(),
        It.IsAny<WorkflowExecutionContext>()),
    Times.Once);



        }
    }
}

