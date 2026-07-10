using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                case NotificationRecipient.WorkspaceMember:
                    return GetRequiredInt(context, "MemberId");

                default:

                    throw new InvalidOperationException(
                        $"Unsupported recipient '{recipient}'.");
            }
        }
        private static int GetRequiredInt(WorkflowExecutionContext context,string key)
        {
            var value=context.GetValue(key);
            if( value is int id){
                return id;
            }
            throw new InvalidOperationException(
        $"Workflow context does not contain a valid '{key}'.");
        }

        public async Task ExecuteAsync(WorkflowAction action, WorkflowExecutionContext executionContext)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            var parameters = JsonSerializer.Deserialize<NotifyUserParameters>(action.Parameters, options);

            Console.WriteLine($"JSON = {action.Parameters}");
            Console.WriteLine($"Recipient = {parameters?.Recipient}");
            Console.WriteLine($"Message = '{parameters?.Message}'");
            if (parameters is null)
            {
                throw new InvalidOperationException(
                    "Invalid NotifyUser parameters.");
            }

            int recipientId =
     ResolveRecipientId(
         parameters.Recipient,
         executionContext);

            await _notificationService.NotifyAsync(
    recipientId,
    parameters.Message,
    NotificationType.Workflow,
    null);
        }
    }
}
