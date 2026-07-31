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
        private readonly Project project = new(
            "DevFlow",
            "Project Description",
            1,
            1);


        private Workflow CreateWorkflow()
        {
            var workflow = new Workflow(
                project,
                1,
                "Assignment Workflow",
                "Notify user",
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
                    1));
            workflow.Enable();

            return workflow;
        }


        private WorkflowExecutionContext CreateContext()
        {
            return new WorkflowExecutionContext(
                new Dictionary<string, object?>
                {
                    { "ProjectId", 1 }
                });
        }



        [Fact]
        public async Task Should_Execute_All_Actions_When_All_Conditions_Pass()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);


            var workflow = CreateWorkflow();


            workflowRepositoryMock
                .Setup(x => x.GetEnabledByTriggerAsync(
                    1,
                    WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow>
                {
                    workflow
                });


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



            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                CreateContext());


            workflowRepositoryMock.Verify(
                x => x.GetEnabledByTriggerAsync(
                    1,
                    WorkflowTrigger.TaskAssigned),
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
                Times.Once);
        }



        [Fact]
        public async Task Should_Not_Execute_Actions_When_Condition_Fails()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);


            workflowRepositoryMock
                .Setup(x => x.GetEnabledByTriggerAsync(
                    1,
                    WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow>
                {
                    CreateWorkflow()
                });


            conditionEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(false);



            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                CreateContext());



            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);
        }



        [Fact]
        public async Task Should_Do_Nothing_When_No_Workflows_Found()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);


            workflowRepositoryMock
                .Setup(x => x.GetEnabledByTriggerAsync(
                    1,
                    WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow>());



            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                CreateContext());



            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Never);
        }



        [Fact]
        public async Task Should_Continue_When_Action_Fails()
        {
            var workflowRepositoryMock = new Mock<IWorkflowRepository>();
            var conditionEvaluatorMock = new Mock<IWorkflowConditionEvaluator>();
            var actionDispatcherMock = new Mock<IActionDispatcher>();

            var engine = new WorkflowEngine(
                workflowRepositoryMock.Object,
                conditionEvaluatorMock.Object,
                actionDispatcherMock.Object);



            workflowRepositoryMock
                .Setup(x => x.GetEnabledByTriggerAsync(
                    1,
                    WorkflowTrigger.TaskAssigned))
                .ReturnsAsync(new List<Workflow>
                {
                    CreateWorkflow()
                });



            conditionEvaluatorMock
                .Setup(x => x.Evaluate(
                    It.IsAny<WorkflowCondition>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .Returns(true);



            actionDispatcherMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()))
                .ThrowsAsync(new Exception());



            await engine.ExecuteAsync(
                WorkflowTrigger.TaskAssigned,
                CreateContext());



            actionDispatcherMock.Verify(
                x => x.ExecuteAsync(
                    It.IsAny<WorkflowAction>(),
                    It.IsAny<WorkflowExecutionContext>()),
                Times.Once);
        }
    }
}