using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging.IntegrationEvents
{
    public record IntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccuredOn { get; init; } = DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName!;
    }
}
