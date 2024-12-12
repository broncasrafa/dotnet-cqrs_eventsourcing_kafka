using CQRS.Core.Events;

namespace Post.Common.Events;


public class PostUpdatedEvent : BaseEvent
{
    public string Message { get; set; }

    public PostUpdatedEvent() : base(nameof(PostUpdatedEvent))
    {
    }
}
