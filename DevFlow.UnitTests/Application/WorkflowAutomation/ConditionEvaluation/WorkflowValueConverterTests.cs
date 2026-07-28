using DevFlow.Application.Workflows.ConditionEvaluation;
using DevFlow.Domain.Enum;
using FluentAssertions;
using Xunit;

namespace DevFlow.UnitTests.Application.WorkflowAutomation.ConditionEvaluation
{
    public class WorkflowValueConverterTests
    {
        [Fact]
        public void Should_Return_String_When_ActualValue_Is_String()
        {
            // Arrange
            var converter = new WorkflowValueConverter();

            // Act
            var result = converter.ConvertToActualType(
                "Hello",
                "World");

            // Assert
            result.Should().Be("World");
        }
        [Fact]
        public void Should_Convert_String_To_Integer()
        {
            //Arrange
            var converter = new WorkflowValueConverter();
            // Act
            var result = converter.ConvertToActualType(
                10,
                "25");
            // Assert
            result.Should().Be(25);
            result.Should().BeOfType<int>();

        }
        [Fact]
        public void Should_Convert_String_To_Boolean()
        {
            // Arrange
            var converter = new WorkflowValueConverter();

            // Act
            var result = converter.ConvertToActualType(
                true,
                "false");

            // Assert
            result.Should().Be(false);
            result.Should().BeOfType<bool>();
        }
        [Fact]
        public void Should_Convert_String_To_Enum()
        {
            // Arrange
            var converter = new WorkflowValueConverter();

            // Act
            var result = converter.ConvertToActualType(
                WorkflowTrigger.TaskAssigned,
                "ProjectCreated");

            // Assert
            result.Should().Be(WorkflowTrigger.ProjectCreated);
            result.Should().BeOfType<WorkflowTrigger>();
        }
        [Fact]
        public void Should_Return_Expected_Value_When_Actual_Value_Is_Null()
        {
            // Arrange
            var converter = new WorkflowValueConverter();

            // Act
            var result = converter.ConvertToActualType(
                null,
                "High");

            // Assert
            result.Should().Be("High");
        }
        [Fact]
        public void Should_Throw_InvalidOperationException_When_Conversion_Fails()
        {
            // Arrange
            var converter = new WorkflowValueConverter();

            // Act
            Action act = () => converter.ConvertToActualType(
                10,
                "abc");

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("*Cannot convert workflow value*");
        }
    }
}
