using CQRS.Core.Events;

namespace Post.Common.Events;


public class PostMessageUpdatedEvent : BaseEvent
{
    public string Message { get; set; }

    public PostMessageUpdatedEvent(Guid id, string message) : base(nameof(PostMessageUpdatedEvent))
    {
        Id = id;
        Message = message;
    }
}
