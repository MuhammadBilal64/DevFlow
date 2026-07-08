using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Workflows.ActionExecution.Models
{
    public class NotifyUserParameters
    {
        public string Recipient { get; init; } = null!;
        public string Message { get; init; } = null!;
    }
}
