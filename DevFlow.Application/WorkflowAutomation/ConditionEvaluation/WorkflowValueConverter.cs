using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class WorkflowValueConverter : IWorkflowValueConverter
    {
        public object? ConvertToActualType(
            object? actualValue,
            string expectedValue)
        {
            if (actualValue is null)
                return expectedValue;

            try
            {
                var targetType = actualValue.GetType();

                if (targetType == typeof(string))
                {
                    return expectedValue;
                }

                if (targetType.IsEnum)
                {
                    return Enum.Parse(
                        targetType,
                        expectedValue,
                        ignoreCase: true);
                }

                return Convert.ChangeType(
                    expectedValue,
                    targetType);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is FormatException ||
                ex is InvalidCastException)
            {
                throw new InvalidOperationException(
                    $"Cannot convert workflow value '{expectedValue}' to type '{actualValue.GetType().Name}'.",
                    ex);
            }
        }
    }
}
