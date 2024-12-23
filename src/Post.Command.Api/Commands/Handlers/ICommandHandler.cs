﻿namespace Post.Command.Api.Commands.Handlers;

public interface ICommandHandler
{
    Task HandleAsync(AddCommentCommand command);
    Task HandleAsync(EditCommentCommand command);
    Task HandleAsync(RemoveCommentCommand command);
    Task HandleAsync(NewPostCommand command);
    Task HandleAsync(EditPostCommand command);
    Task HandleAsync(LikePostCommand command);
    Task HandleAsync(DeletePostCommand command);
}