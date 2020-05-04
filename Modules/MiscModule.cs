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
	public class MiscModule : BaseCommandModule {
      [Command("ping")]
        [Description("Showing information about the bot.")]
        public async Task Info(CommandContext ctx)
        {
          String ping = ctx.Client.Ping.ToString();
          await ctx.RespondAsync(ping + " ms");
          await ctx.Message.DeleteAsync();
        }
	}
}
