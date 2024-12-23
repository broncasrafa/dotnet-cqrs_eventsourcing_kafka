using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Confluent.Kafka;

namespace Post.Command.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ILogger<EventProducer> _logger;
    private readonly ProducerConfig _config;

    public EventProducer(IOptions<ProducerConfig> config, ILogger<EventProducer> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
            //Timestamp = new Timestamp(DateTimeOffset.UtcNow)
        };

        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            throw new Exception($"Could not produce {@event.GetType().Name} message to topic - '{topic}' due to the following reason: {deliveryResult.Message}.");

        _logger.LogInformation($"Produced {@event.GetType().Name} message to topic - '{topic}' with key - '{eventMessage.Key}'.");
    }
}

/*
 Kafka Topic: Um tópico do Kafka pode ser visto como um canal (channel) através do qual dados de eventos são transmitidos (streamed). 
              Produtores (producers) sempre enviam ou produzem mensagens para um tópico, enquanto consumidores (consumers) consomem 
              eventos dos tópicos aos quais se inscrevem.
 
 */