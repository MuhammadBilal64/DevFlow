using System.Collections.ObjectModel;

namespace DevFlow.Application.Common.Models;

public sealed class WorkflowExecutionContext
{
    private readonly IReadOnlyDictionary<string, object?> _values;

    public WorkflowExecutionContext(
        IDictionary<string, object?> values)
    {
        _values = new ReadOnlyDictionary<string, object?>(values);
    }

    public object? GetValue(string field)
    {
        _values.TryGetValue(field, out var value);
        return value;
    }

    public bool Contains(string field)
    {
        return _values.ContainsKey(field);
    }
}