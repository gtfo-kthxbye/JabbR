using System;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
    using System.IO;
    using System.Text;

    [Command("code", "Code_CommandInfo", "[code]", "user")]
    public class CodeCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            string code = String.Join(" ", args).Trim();

            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            room.EnsureOpen();
            if (args.Length > 0)
            {
                context.NotificationService.DisplayHtml(callingUser, room, string.Format("<pre class=\"prettyprint\">{0}</pre><script type=\"text/javascript\">prettyPrint();</script>", code));
            }
        }
    }
}