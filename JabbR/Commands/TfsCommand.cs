using System;
using System.Linq;
using JabbR.Models;
using JabbR.Services;
using JabbR.Tfs;

namespace JabbR.Commands
{
    [Command("tfs", "Tfs_CommandInfo", "[#workitem_Id]", "user")]
    public class TfsCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            room.EnsureOpen();

            if (args.Length > 0)
            {
                if (args[0].StartsWith("#"))
                {
                    var id = 0;
                    if (int.TryParse(args[0].Replace("#", string.Empty), out id))
                    {
                        var html = new TfsToHtml().GetWorkItemHtml(id);
                        context.NotificationService.DisplayHtml(callingUser, room, html);
                    }                    
                }
            }            
        }
    }
}