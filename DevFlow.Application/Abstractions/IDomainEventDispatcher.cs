using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Events;

namespace DevFlow.Application.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task PublishAsync(IDomainEvent domainEvent);
    }
}
