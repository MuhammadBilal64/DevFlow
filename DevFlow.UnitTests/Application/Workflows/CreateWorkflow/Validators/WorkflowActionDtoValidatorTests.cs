using DevFlow.Application.Workflows.CreateWorkflow.Validators;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.CreateWorkflow.Validators
{
    public class WorkflowActionDtoValidatorTests
    {
        private readonly WorkflowActionDtoValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Action_Is_Valid()
        {
            // Arrange
            var dto = new WorkflowActionDto
            {
                ActionType = WorkflowActionType.NotifyUser,
                Parameters = "UserId=1",
                Order = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData((WorkflowActionType)100)]
        [InlineData((WorkflowActionType)(-1))]
        public void Should_Have_Error_When_ActionType_Is_Invalid(WorkflowActionType actionType)
        {
            // Arrange
            var dto = new WorkflowActionDto
            {
                ActionType = actionType,
                Parameters = "UserId=1",
                Order = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActionType);
        }

        [Fact]
        public void Should_Have_Error_When_Parameters_Are_Empty()
        {
            // Arrange
            var dto = new WorkflowActionDto
            {
                ActionType = WorkflowActionType.NotifyUser,
                Parameters = "",
                Order = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Parameters);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_Order_Is_Invalid(int order)
        {
            // Arrange
            var dto = new WorkflowActionDto
            {
                ActionType = WorkflowActionType.NotifyUser,
                Parameters = "UserId=1",
                Order = order
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Order);
        }
    }
}