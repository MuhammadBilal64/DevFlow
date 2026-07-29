using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Domain.Workflows
{
    public class WorkflowsTests
    {
        [Fact]
        public void Should_Create_Workflow_With_Valid_Data()
        {
            // Arrange
            var name = "Task Assignment Workflow";
            var description = "Assign tasks automatically";
            var trigger = WorkflowTrigger.TaskAssigned;

            // Act
            var workflow = new Workflow(
                name,
                description,
                trigger);

            // Assert
            workflow.Name.Should().Be(name);
            workflow.Description.Should().Be(description);
            workflow.Trigger.Should().Be(trigger);
            workflow.IsEnabled.Should().BeFalse();
        }
        [Fact]
        public void Should_Throw_When_Name_Is_Empty()
        {
            // Arrange
            var name = "";

            // Act
            Action act = () => new Workflow(
                name,
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Workflow name is required.*")
                .And.ParamName.Should().Be("name");
        }
        [Fact]
        public void Should_Enable_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Task Assignment Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            workflow.Enable();

            // Assert
            workflow.IsEnabled.Should().BeTrue();
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Not_Enable_When_Already_Enabled()
        {
            // Arrange
            var workflow = new Workflow(
                "Task Assignment Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflow.Enable();

            var updatedAt = workflow.UpdatedAt;

            // Act
            workflow.Enable();

            // Assert
            workflow.IsEnabled.Should().BeTrue();
            workflow.UpdatedAt.Should().Be(updatedAt);
        }
        [Fact]
        public void Should_Disable_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Task Assignment Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflow.Enable();

            // Act
            workflow.Disable();

            // Assert
            workflow.IsEnabled.Should().BeFalse();
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Not_Disable_When_Already_Disabled()
        {
            // Arrange
            var workflow = new Workflow(
                "Task Assignment Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            workflow.Disable();

            // Assert
            workflow.IsEnabled.Should().BeFalse();
            workflow.UpdatedAt.Should().BeNull();
        }
        [Fact]
        public void Should_Update_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Old Workflow",
                "Old Description",
                WorkflowTrigger.TaskAssigned);

            var newName = "New Workflow";
            var newDescription = "New Description";

            // Act
            workflow.Update(
                newName,
                newDescription);

            // Assert
            workflow.Name.Should().Be(newName);
            workflow.Description.Should().Be(newDescription);
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Throw_When_Updating_With_Empty_Name()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            Action act = () => workflow.Update(
                "",
                "New Description");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Workflow name is required.*")
                .And.ParamName.Should().Be("name");
        }
        [Fact]
        public void Should_Add_Action()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            // Act
            workflow.AddAction(action);

            // Assert
            workflow.Actions.Should().Contain(action);
            workflow.Actions.Should().HaveCount(1);
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Throw_When_Action_Is_Null()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            Action act = () => workflow.AddAction(null!);

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("workflowAction");
        }
        [Fact]
        public void Should_Throw_When_Adding_Action_To_Enabled_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflow.Enable();

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            // Act
            Action act = () => workflow.AddAction(action);

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot modify an enabled workflow.");
        }
        [Fact]
        public void Should_Remove_Action()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            workflow.AddAction(action);

            // Act
            workflow.RemoveAction(action);

            // Assert
            workflow.Actions.Should().BeEmpty();
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Throw_When_Removing_Null_Action()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            Action act = () => workflow.RemoveAction(null!);

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("workflowAction");
        }
        [Fact]
        public void Should_Throw_When_Removing_Action_From_Enabled_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var action = new WorkflowAction(
                WorkflowActionType.NotifyUser,
                "parameters",
                1);

            workflow.AddAction(action);
            workflow.Enable();

            // Act
            Action act = () => workflow.RemoveAction(action);

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot modify an enabled workflow.");
        }
        [Fact]
        public void Should_Add_Condition()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            // Act
            workflow.AddCondition(condition);

            // Assert
            workflow.Conditions.Should().Contain(condition);
            workflow.Conditions.Should().HaveCount(1);
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Throw_When_Condition_Is_Null()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            Action act = () => workflow.AddCondition(null!);

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("workflowCondition");
        }
        [Fact]
        public void Should_Throw_When_Adding_Condition_To_Enabled_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            workflow.Enable();

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            // Act
            Action act = () => workflow.AddCondition(condition);

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("cannot modify an enabled workflow");
        }
        [Fact]
        public void Should_Remove_Condition()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            workflow.AddCondition(condition);

            // Act
            workflow.RemoveCondition(condition);

            // Assert
            workflow.Conditions.Should().BeEmpty();
            workflow.UpdatedAt.Should().NotBeNull();
        }
        [Fact]
        public void Should_Throw_When_Removing_Null_Condition()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            // Act
            Action act = () => workflow.RemoveCondition(null!);

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("workflowCondition");
        }
        [Fact]
        public void Should_Throw_When_Removing_Condition_From_Enabled_Workflow()
        {
            // Arrange
            var workflow = new Workflow(
                "Workflow",
                "Description",
                WorkflowTrigger.TaskAssigned);

            var condition = new WorkflowCondition(
                "Priority",
                WorkflowOperator.Equals,
                "High");

            workflow.AddCondition(condition);
            workflow.Enable();

            // Act
            Action act = () => workflow.RemoveCondition(condition);

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot modify an enabled workflow.");
        }


    }
}
