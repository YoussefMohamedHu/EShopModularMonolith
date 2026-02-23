using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Data.Processors
{
    public class OutboxProcessor(
        IServiceProvider serviceProvider,
        IBus bus,
        ILogger<OutboxProcessor> logger
        ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                    var outboxMessages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedOn == null)
                        .ToListAsync(stoppingToken);

                    foreach (var message in outboxMessages)
                    {
                        var messageType = Type.GetType(message.Type);
                        if (messageType is null)
                        {
                            logger.LogError("Message type {MessageType} not found.", message.Type);
                            continue;
                        }
                        var deserializedMessage = System.Text.Json.JsonSerializer.Deserialize(message.Payload, messageType);
                        if (deserializedMessage is null)
                        {
                            logger.LogError("Failed to deserialize message with id {MessageId}.", message.Id);
                            continue;
                        }
                        await bus.Publish((dynamic)deserializedMessage, stoppingToken);

                        message.MarkAsProcessed();

                        logger.LogInformation("Processed outbox message with id {MessageId}.", message.Id);

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }

                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing outbox messages.");
                }
            }
        }
    }
}
