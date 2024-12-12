using CQRS.Core.Commands;

namespace Post.Command.Api.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommendId { get; set; }
    public string Username { get; set; }
}
