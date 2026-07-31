using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Workflows
{
    public class WorkflowsTests
    {
        private static Workflow CreateWorkflow(
            string name = "Workflow",
            string description = "Description")
        {
            var project = new Project(
                "Test Project",
                "Test Project Description",
                1,
                1);

            return new Workflow(
                project,
                1,
                name,
                description,
                WorkflowTrigger.TaskAssigned);
        }

        [Fact]
        public void Should_Create_Workflow_With_Valid_Data()
        {
            // Arrange
            var name = "Task Assignment Workflow";
            var description = "Assign tasks automatically";

            // Act
            var workflow = CreateWorkflow(name, description);

            // Assert
            workflow.Name.Should().Be(name);
            workflow.Description.Should().Be(description);
            workflow.Trigger.Should().Be(WorkflowTrigger.TaskAssigned);
            workflow.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void Should_Throw_When_Name_Is_Empty()
        {
            Action act = () => CreateWorkflow("");

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Workflow name is required.*")
                .And.ParamName.Should().Be("name");
        }

        [Fact]
        public void Should_Enable_Workflow()
        {
            var workflow = CreateWorkflow();

            workflow.Enable();

            workflow.IsEnabled.Should().BeTrue();
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Not_Enable_When_Already_Enabled()
        {
            var workflow = CreateWorkflow();

            workflow.Enable();

            var updatedAt = workflow.UpdatedAt;

            workflow.Enable();

            workflow.IsEnabled.Should().BeTrue();
            workflow.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void Should_Disable_Workflow()
        {
            var workflow = CreateWorkflow();

            workflow.Enable();
            workflow.Disable();

            workflow.IsEnabled.Should().BeFalse();
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Not_Disable_When_Already_Disabled()
        {
            var workflow = CreateWorkflow();

            workflow.Disable();

            workflow.IsEnabled.Should().BeFalse();
            workflow.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Should_Update_Workflow()
        {
            var workflow = CreateWorkflow();

            workflow.Update(
                "New Workflow",
                "New Description");

            workflow.Name.Should().Be("New Workflow");
            workflow.Description.Should().Be("New Description");
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Updating_With_Empty_Name()
        {
            var workflow = CreateWorkflow();

            Action act = () => workflow.Update("", "New Description");

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Workflow name is required.*")
                .And.ParamName.Should().Be("name");
        }

        [Fact]
        public void Should_Add_Action()
        {
            var workflow = CreateWorkflow();

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            workflow.AddAction(action);

            workflow.Actions.Should().Contain(action);
            workflow.Actions.Should().HaveCount(1);
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Action_Is_Null()
        {
            var workflow = CreateWorkflow();

            Action act = () => workflow.AddAction(null!);

            act.Should()
                .Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("action");
        }

        [Fact]
        public void Should_Throw_When_Adding_Action_To_Enabled_Workflow()
        {
            var workflow = CreateWorkflow();

            workflow.Enable();

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            Action act = () => workflow.AddAction(action);

            act.Should()
                .Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Remove_Action()
        {
            var workflow = CreateWorkflow();

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            workflow.AddAction(action);

            workflow.RemoveAction(action);

            workflow.Actions.Should().BeEmpty();
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Add_Condition()
        {
            var workflow = CreateWorkflow();

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            workflow.AddCondition(condition);

            workflow.Conditions.Should().Contain(condition);
            workflow.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Remove_Condition()
        {
            var workflow = CreateWorkflow();

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            workflow.AddCondition(condition);

            workflow.RemoveCondition(condition);

            workflow.Conditions.Should().BeEmpty();
            workflow.UpdatedAt.Should().NotBeNull();
        }
    }
}