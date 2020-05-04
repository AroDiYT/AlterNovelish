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
    [Group("answer"), Aliases("question")]
	[Description("Bot decides yes or no")]
	public class AnswerModule : BaseCommandModule {
        private Random _rng = new Random();
        [GroupCommand]
        public async Task Answer(CommandContext ctx, [RemainingText, Description("Your question.")] String Question)
        {
          String[] arr = new[] {"yes", "no"};
          int r = _rng.Next(2);
           var embeds = new DiscordEmbedBuilder
                {
                    Title = Question,
                    Description = "**`" + arr[r] + "`**",
                    Color = DiscordColor.Lilac
                };
          await ctx.RespondAsync(embed : embeds);
        }
    }
}
		