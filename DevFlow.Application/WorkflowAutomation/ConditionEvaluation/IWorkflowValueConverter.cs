using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Workflows.ConditionEvaluation
{
    public interface IWorkflowValueConverter
    {
        object? ConvertToActualType(
      object? actualValue,
      string expectedValue);
    }
}
