using System;
using System.Threading.Tasks;
using System.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql;

namespace BotTemplate.Modules {
    [Group("Channel")]
	[Description("View if rp channel")]
	public class ChannelModule : BaseCommandModule {
        [GroupCommand]
        public async Task ViewAsync(CommandContext ctx, DiscordChannel channel) {
			var cc = await ChannelManager.GetAsync(channel.Id);
            if(cc == null)
            {
                await ctx.RespondAsync($"{channel.Mention} is a normal Channel.");
            }
            else
            {
                if(cc.RP == false) 
                {
                    await ctx.RespondAsync($"{channel.Mention} is a normal Channel.");
                }
                else
                {
                    await ctx.RespondAsync($"{channel.Mention} is an RP channel");
                }
            }
		}
        [Command("rp")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task RPasync(CommandContext ctx, DiscordChannel channel)
        {
            var obj = new Channel()
            {
                ID = channel.Id,
                RP = true
            };
            await ChannelManager.SyncAsync(obj);
            await ctx.RespondAsync($"{channel.Mention} is now an RP channel");
        }
    }
}
		