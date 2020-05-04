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
    [Group("say"), Aliases("speak")]
	[Description("Talk as the bot.")]
	public class SpeakModule : BaseCommandModule {
        [GroupCommand()]
        public async Task Say(CommandContext ctx, [RemainingText, Description("The text Novelish should say.")] String content)
        {
          await ctx.TriggerTypingAsync();
          await ctx.RespondAsync(content);
          await ctx.Message.DeleteAsync();
        }
        [Command("to")]
        [Description("Speak to a specific channel")]
        public async Task SayTo(CommandContext ctx, [Description("Channel to where the text should go.")]DiscordChannel channel,
        [RemainingText, Description("Text which should be said.")] string text)
        {
          await channel.TriggerTypingAsync();
          await channel.SendMessageAsync(text);
          await ctx.Message.DeleteAsync();
        }
    }
}
		