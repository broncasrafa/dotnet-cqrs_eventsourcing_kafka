using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent
{
    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;


    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }
}