using DevFlow.Application.Workflows.CreateWorkflow.Validators;
using DevFlow.Application.Workflows.WorkflowDtos;
using DevFlow.Domain.Enum;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Workflows.CreateWorkflow.Validators
{
    public class WorkflowConditionDtoValidatorTests
    {
        private readonly WorkflowConditionDtoValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Condition_Is_Valid()
        {
            var dto = new WorkflowConditionDto
            {
                Field = "Priority",
                Operator = WorkflowOperator.Equals,
                Value = "High"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Field_Is_Empty()
        {
            var dto = new WorkflowConditionDto
            {
                Field = "",
                Operator = WorkflowOperator.Equals,
                Value = "High"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Field);
        }

        [Fact]
        public void Should_Have_Error_When_Field_Exceeds_Maximum_Length()
        {
            var dto = new WorkflowConditionDto
            {
                Field = new string('A', 101),
                Operator = WorkflowOperator.Equals,
                Value = "High"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Field);
        }

        [Theory]
        [InlineData((WorkflowOperator)100)]
        [InlineData((WorkflowOperator)(-1))]
        public void Should_Have_Error_When_Operator_Is_Invalid(WorkflowOperator op)
        {
            var dto = new WorkflowConditionDto
            {
                Field = "Priority",
                Operator = op,
                Value = "High"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Operator);
        }

        [Fact]
        public void Should_Have_Error_When_Value_Is_Empty()
        {
            var dto = new WorkflowConditionDto
            {
                Field = "Priority",
                Operator = WorkflowOperator.Equals,
                Value = ""
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Value);
        }

        [Fact]
        public void Should_Have_Error_When_Value_Exceeds_Maximum_Length()
        {
            var dto = new WorkflowConditionDto
            {
                Field = "Priority",
                Operator = WorkflowOperator.Equals,
                Value = new string('A', 301)
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Value);
        }
    }
}