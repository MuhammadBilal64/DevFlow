using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Abstractions
{
    public interface INotificationService
    {

        Task NotifyAsync(
    int userId,
    string message,
    NotificationType type,
    int? referenceId = null);
    }
}
