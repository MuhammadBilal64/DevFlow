using DevFlow.Application.Workflows.CreateWorkflow;
using DevFlow.Application.Workflows.CreateWorkflow.Validators;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.CreateWorkflow.Validators
{
    public class CreateWorkflowValidatorTests
    {
        private readonly CreateWorkflowValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateWorkflowCommand
            {
                Name = "Task Assignment",
                Description = "Workflow Description",
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
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreateWorkflowCommand
            {
                Name = ""
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
        {
            var command = new CreateWorkflowCommand
            {
                Name = new string('A', 101)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
                Description = new string('A', 501)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData((WorkflowTrigger)100)]
        [InlineData((WorkflowTrigger)(-1))]
        public void Should_Have_Error_When_Trigger_Is_Invalid(WorkflowTrigger trigger)
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
                Trigger = trigger
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Trigger);
        }

        [Fact]
        public void Should_Have_Error_When_Actions_Are_Empty()
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
                Conditions =
                {
                    new WorkflowConditionDto
                    {
                        Field = "Priority",
                        Operator = WorkflowOperator.Equals,
                        Value = "High"
                    }
                }
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Actions);
        }

        [Fact]
        public void Should_Have_Error_When_Conditions_Are_Null()
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
                Conditions = null!,
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

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Conditions);
        }

        [Fact]
        public void Should_Have_Error_When_Condition_Dto_Is_Invalid()
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
                Conditions =
                {
                    new WorkflowConditionDto()
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

            var result = _validator.TestValidate(command);

            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public void Should_Have_Error_When_Action_Dto_Is_Invalid()
        {
            var command = new CreateWorkflowCommand
            {
                Name = "Workflow",
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
                    new WorkflowActionDto()
                }
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveAnyValidationError();
        }
    }
}