using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Command.Domain.Aggregates;

namespace Post.Command.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }


    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException("Incorrect post ID provided");

        return eventStream.OrderBy(c => c.Version).Select(c => c.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion) // && eventStream.Any() && eventStream.Last().Version != expectedVersion
            throw new ConcurrencyException();

        var version = expectedVersion;

        foreach(var @event in events)
        {
            version++;

            @event.Version = version;

            var eventModel = new EventModel(
                aggregateId: aggregateId,
                aggregateType: nameof(PostAggregate),
                version: version,
                eventType: @event.GetType().Name,
                eventData: @event
            );

            await _eventStoreRepository.SaveAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic, @event);
        }
    }
}

/*
• Por que a lógica de negócios do EventStore implementa o controle de simultaneidade otimista?
► Para que o estado do Aggregate seja reproduzido/recriado corretamente, é importante que a ordem dos eventos seja 
imposta pela implementação do versionamento de eventos. Para que os eventos sejam armazenados no Event Store na 
ordem ou sequência correta. O controle de simultaneidade otimista é então usado para garantir que apenas as versões 
de eventos esperadas possam ser persistidas no event store. Isso é especialmente importante se duas ou mais 
solicitações de cliente forem feitas simultaneamente para alterar o estado do agregado.


• O que é um Event Store?
► Um Event Store é um banco de dados usado para armazenar dados como uma sequência de eventos imutáveis ​​ao longo do tempo.
O Event Store é usado no lado de gravação ou comando de um aplicativo baseado em CQRS e Event Sourcing, e é usado para 
armazenar dados como uma sequência de eventos imutáveis ​​ao longo do tempo.
 */
