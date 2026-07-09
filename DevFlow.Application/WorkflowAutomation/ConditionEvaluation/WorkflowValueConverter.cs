using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public class WorkflowValueConverter : IWorkflowValueConverter
    {
        public object? ConvertToActualType(object? actualValue, string expectedValue)
        {
            if (actualValue is null)
                return expectedValue;

            var targetType = actualValue.GetType();
            if (targetType == typeof(string))
            {
                return expectedValue;
            }
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, expectedValue,ignoreCase:true);
            }
            return Convert.ChangeType(
    expectedValue,
    targetType);
        }
    }
}
