using CQRS.Core.Events;

namespace Post.Common.Events;
public class CommentRemovedEvent : BaseEvent
{
    public Guid CommentId { get; set; }


    public CommentRemovedEvent(Guid id, Guid commentId) : base(nameof(CommentRemovedEvent))
    {
        Id = id;
        CommentId = commentId;
    }
}
