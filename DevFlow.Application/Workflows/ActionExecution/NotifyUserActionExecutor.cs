using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Workflows.ActionExecution.Models;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Workflows.ActionExecution
{
    public class NotifyUserActionExecutor : IActionExecutor
    {
        public WorkflowActionType Type => WorkflowActionType.NotifyUser;
        private readonly INotificationService _notificationService;
        public NotifyUserActionExecutor(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        private int ResolveRecipientId(
      NotificationRecipient recipient,
      WorkflowExecutionContext context)
        {
            switch (recipient)
            {
                case NotificationRecipient.Assignee:

                    return GetRequiredInt(
                        context,
                        "AssigneeId");

                case NotificationRecipient.Reporter:

                    return GetRequiredInt(
                        context,
                        "ReporterId");

                case NotificationRecipient.ProjectManager:

                    return GetRequiredInt(
                        context,
                        "ProjectManagerId");

                case NotificationRecipient.Creator:

                    return GetRequiredInt(
                        context,
                        "CreatorId");

                default:

                    throw new InvalidOperationException(
                        $"Unsupported recipient '{recipient}'.");
            }
        }
        private static int GetRequiredInt(WorkflowExecutionContext context,string key)
        {
            var value=context.GetValue(key);
            if( value is int id){
                return (int)value;
            }
            throw new InvalidOperationException(
        $"Workflow context does not contain a valid '{key}'.");
        }

        public async Task ExecuteAsync(WorkflowAction action, WorkflowExecutionContext executionContext)
        {
            var parameters = JsonSerializer.Deserialize<NotifyUserParameters>(
    action.Parameters);

            if (parameters is null)
            {
                throw new InvalidOperationException(
                    "Invalid NotifyUser parameters.");
            }
            if (!Enum.TryParse<NotificationRecipient>(
        parameters.Recipient,
        true,
        out var recipient))
            {
                throw new InvalidOperationException(
                    $"Invalid notification recipient '{parameters.Recipient}'.");
            }

            int recipientId =
     ResolveRecipientId(
         recipient,
         executionContext);

            await _notificationService.NotifyAsync(
    recipientId,
    parameters.Message,
    NotificationType.Workflow,
    null);
        }
    }
}
