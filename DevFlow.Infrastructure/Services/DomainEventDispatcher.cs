using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Events;
using MediatR;

namespace DevFlow.Infrastructure.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator=mediator;
        }
        public async Task PublishAsync(IDomainEvent domainEvent)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
