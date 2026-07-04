using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Domain.Entities
{
    public class Notification
    {
        public int Id { get;private set; }
        public int UserId {  get; private set; }
        public string Message { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; } = false;
        public DateTime CreatedAt { get; private set; }
        public int ?ReferenceId {  get;  private set; }
        public User User { get; private set; } = null!;

        public Notification(
     int userId,
     string message,
     NotificationType type,
     int? referenceId = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user id.", nameof(userId));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(
     "Message cannot be empty.",
     nameof(message));

            if (referenceId.HasValue && referenceId.Value <= 0)
                throw new ArgumentException(
     "ReferenceId must be greater than zero.",
     nameof(referenceId));

            UserId = userId;
            Message = message;
            Type = type;
            ReferenceId = referenceId;
            CreatedAt = DateTime.UtcNow;
        }
        public void MarkAsRead()
        {
            if (IsRead)
                return;

            IsRead = true;
        }
        private Notification()
        {
        }

    }
}
