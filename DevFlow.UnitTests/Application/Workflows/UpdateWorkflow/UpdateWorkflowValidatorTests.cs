using DevFlow.Application.Workflows.UpdateWorkflow;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.UpdateWorkflow
{
    public class UpdateWorkflowValidatorTests
    {
        private readonly UpdateWorkflowValidator _validator = new();


        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = "Task Assignment Workflow",
                Description = "Workflow Description",

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
            var result = _validator.TestValidate(command);


            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_ProjectId_Is_Invalid(int projectId)
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = projectId,
                WorkflowId = 1,
                Name = "Workflow"
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.ProjectId);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_WorkflowId_Is_Invalid(int workflowId)
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = workflowId,
                Name = "Workflow"
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.WorkflowId);
        }



        [Theory]
        [InlineData("")]
        public void Should_Have_Error_When_Name_Is_Invalid(string name)
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = name
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.Name);
        }



        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = new string('A', 101)
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.Name);
        }



        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = "Workflow",
                Description = new string('A', 501)
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.Description);
        }



        [Fact]
        public void Should_Have_Error_When_Action_Is_Invalid()
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = "Workflow",

                Actions =
                {
                    new WorkflowActionDto
                    {
                        ActionType = WorkflowActionType.NotifyUser,
                        Parameters = "",
                        Order = 0
                    }
                }
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor("Actions[0].Parameters");
            result.ShouldHaveValidationErrorFor("Actions[0].Order");
        }



        [Fact]
        public void Should_Have_Error_When_Condition_Is_Invalid()
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = "Workflow",

                Conditions =
                {
                    new WorkflowConditionDto
                    {
                        Field = "",
                        Operator = WorkflowOperator.Equals,
                        Value = ""
                    }
                }
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor("Conditions[0].Field");
            result.ShouldHaveValidationErrorFor("Conditions[0].Value");
        }



        [Fact]
        public void Should_Have_Error_When_Action_Order_Is_Duplicated()
        {
            var command = new UpdateWorkflowCommand
            {
                ProjectId = 1,
                WorkflowId = 1,
                Name = "Workflow",

                Actions =
                {
                    new WorkflowActionDto
                    {
                        ActionType = WorkflowActionType.NotifyUser,
                        Parameters = "UserId=1",
                        Order = 1
                    },

                    new WorkflowActionDto
                    {
                        ActionType = WorkflowActionType.NotifyUser,
                        Parameters = "UserId=2",
                        Order = 1
                    }
                }
            };


            var result = _validator.TestValidate(command);


            result.ShouldHaveValidationErrorFor(x => x.Actions);
        }
    }
}