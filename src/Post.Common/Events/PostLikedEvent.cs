using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostLikedEvent : BaseEvent
{

    public PostLikedEvent(Guid id) : base(nameof(PostLikedEvent))
    {
        Id = id;
    }
}
