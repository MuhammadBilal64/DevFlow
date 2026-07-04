using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Common.Models
{
    public class NotificationRealtimeModel
    {
        public int UserId { get; set; }

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public int? ReferenceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
