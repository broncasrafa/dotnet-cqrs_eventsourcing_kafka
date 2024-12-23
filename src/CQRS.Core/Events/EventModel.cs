using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events;

public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; }
    public BaseEvent EventData { get; set; }

    //public EventModel()
    //{
    //    TimeStamp = DateTime.UtcNow;
    //}

    public EventModel(Guid aggregateId, string aggregateType, int version, string eventType, BaseEvent eventData)
    {
        TimeStamp = DateTime.UtcNow;
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        Version = version;
        EventType = eventType;
        EventData = eventData;
    }
}
