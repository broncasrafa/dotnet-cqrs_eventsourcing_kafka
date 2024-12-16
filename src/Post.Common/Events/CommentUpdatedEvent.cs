using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentUpdatedEvent : BaseEvent
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime CommentUpdatedDate { get; set; } = DateTime.UtcNow;


    public CommentUpdatedEvent(Guid id, Guid commentId, string comment, string username) : base(nameof(CommentUpdatedEvent))
    {
        Id = id;
        CommentId = commentId;
        Comment = comment;
        Username = username;
        CommentUpdatedDate = DateTime.UtcNow;
    }
}
