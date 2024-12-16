using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Command.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {
    }
    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent(id: id, author: author, message: message));
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void Apply(PostMessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }
    
    public void Apply(CommentRemovedEvent @event) 
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void Apply(PostRemovedEvent @event) 
    {
        _id = @event.Id;
        _active = false;
    }

    public void EditMessage(string message)
    {
        if (!_active)
            throw new InvalidOperationException("You cannot edit the message of an inactive post");

        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}");

        RaiseEvent(new PostMessageUpdatedEvent(id: _id, message: message));
    }

    public void LikePost()
    {
        if (!_active)
            throw new InvalidOperationException("You cannot like an inactive post");

        RaiseEvent(new PostLikedEvent(id: _id));
    }

    public void AddComment(string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You cannot add a comment to an inactive post");

        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}");

        RaiseEvent(new CommentAddedEvent(id: _id, commentId: Guid.NewGuid(), comment: comment, username: username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You cannot edit a comment to an inactive post");

        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}");

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException($"You are not allowed to edit a comment that was made by another user");

        RaiseEvent(new CommentUpdatedEvent(id: _id, commentId: commentId, comment: comment, username: username));
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You cannot remove a comment to an inactive post");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}");

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException($"You are not allowed to remove a comment that was made by another user");

        RaiseEvent(new CommentRemovedEvent(id: _id, commentId: commentId));
    }

    public void DeletePost(string username)
    {
        if (!_active)
            throw new InvalidOperationException("The post has already been removed");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}");

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else");

        RaiseEvent(new PostRemovedEvent(id: _id));
    }
}
/*
A validação deve sempre ocorrer antes que o Aggregate levante (raise) um evento, porque um cliente pode passar 
informações incorretas que não queremos que afetem o estado do Aggregate. Uma vez que um evento tenha sido levantado (raised), 
ele será aplicado ao Aggregate e persistido no armazenamento de eventos. Devemos proteger o armazenamento de eventos 
de eventos que contenham erros ou dados não validados.
*/