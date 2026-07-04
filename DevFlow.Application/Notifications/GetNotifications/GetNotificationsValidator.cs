using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Common.Validators;
using FluentValidation;

namespace DevFlow.Application.Notifications.GetNotifications
{
    public class GetNotificationsValidator:AbstractValidator<GetNotificationsQuery>
    {
        public GetNotificationsValidator() { 
        Include(new PaginationRequestValidator());
        }
    }
}
