using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ActionExecution.Models
{
    public class NotifyUserParameters
    {
        public NotificationRecipient Recipient { get; init; }
        public string Message { get; init; } = null!;
    }
}
