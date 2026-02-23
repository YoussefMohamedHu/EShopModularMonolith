using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Models
{
    public class OutboxMessage : Entity<Guid>
    {
        public string Type { get; private set; } = default!;
        public string Payload { get; private set; } = default!;
        public DateTime OccurredOn { get; private set; } = default!;
        public DateTime? ProcessedOn { get; private set; } = default!;
        public OutboxMessage(string type, string payload)
        {
            Id = Guid.NewGuid();
            Type = type;
            Payload = payload;
            OccurredOn = DateTime.UtcNow;
        }
        public void MarkAsProcessed()
        {
            ProcessedOn = DateTime.UtcNow;
        }
    }
}
