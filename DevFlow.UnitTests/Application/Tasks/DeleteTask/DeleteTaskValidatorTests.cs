using DevFlow.Application.Tasks.DeleteTask;
using FluentValidation.TestHelper;
using Xunit;

namespace DevFlow.UnitTests.Application.Tasks.DeleteTask
{
    public class DeleteTaskValidatorTests
    {
        private readonly DeleteTaskValidator _validator = new();

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void Should_Not_Have_Error_When_TaskId_Is_Valid(int taskId)
        {
            var command = new DeleteTaskCommand
            {
                TaskId = taskId
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_TaskId_Is_Invalid(int taskId)
        {
            var command = new DeleteTaskCommand
            {
                TaskId = taskId
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }
    }
}