using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentAddedEvent : BaseEvent
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime CommentDate { get; set; } = DateTime.UtcNow;


    public CommentAddedEvent(Guid id, Guid commentId, string comment, string username) : base(nameof(CommentAddedEvent))
    {
        Id = id;
        CommentId = commentId;
        Comment = comment;
        Username = username;
        CommentDate = DateTime.UtcNow;
    }
}
