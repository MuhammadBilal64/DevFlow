using DevFlow.Api.Contracts.Responses;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Models;
using DevFlow.Application.Notifications.GetNotifications;
using DevFlow.Application.Notifications.GetUnreadNotificationCount;
using DevFlow.Application.Notifications.MarkAllNotificationsAsRead;
using DevFlow.Application.Notifications.MarkNotificationAsRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromQuery]GetNotificationsQuery query)
        {
            
            var result=await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<GetNotificationsResult>>.Ok(result, "Notifications Retrieved Successfully"));
        }
        [HttpGet("unread-count")]
        [Authorize]
        public async Task<IActionResult> GetUnReadCount()
        {
            var result = await _mediator.Send(new GetUnreadNotificationCountQuery { });
            return Ok(ApiResponse<GetUnreadNotificationCountResult>.Ok(result, "Count Retrieved Successfully"));
        }
        [HttpPut("{notificationId}/read")]
        [Authorize]
        public async Task<IActionResult>MarkNotificationAsRead([FromRoute]int notificationId)
        {
            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = notificationId
            };
            var result=await _mediator.
                Send(command);
            return Ok(ApiResponse<MarkNotificationAsReadResult>.Ok(result, "Notification marked as read successfully"));
        }
        [HttpPut("read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var result = await _mediator.Send(new MarkAllNotificationsAsReadCommand { });
            return Ok(ApiResponse<MarkAllNotificationsAsReadResult>.Ok(result, "All Notification marked as read Successfully "));

        }

    }
}
