using Microsoft.Extensions.Options;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using Post.Command.Infrastructure.Configurations;
using MongoDB.Driver;

namespace Post.Command.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> configuration)
    {
        var mongoClient = new MongoClient(configuration.Value.ConnectionString);
        var database = mongoClient.GetDatabase(configuration.Value.DatabaseName);

        _eventStoreCollection = database.GetCollection<EventModel>(configuration.Value.CollectionName);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(e => e.AggregateId == aggregateId).ToListAsync().ConfigureAwait(false); 
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }
}
