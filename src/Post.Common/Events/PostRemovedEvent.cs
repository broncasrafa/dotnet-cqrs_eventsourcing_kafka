using CQRS.Core.Events;

namespace Post.Common.Events;


public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent(Guid id) : base(nameof(PostRemovedEvent))
    {
        Id = id;
    }
}
