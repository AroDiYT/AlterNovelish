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
    [Group("currency"), Aliases("cur")]
	[Description("The Currency System")]
	public class CurrencyModule : BaseCommandModule {
        private Random _rng = new Random();
        [GroupCommand]
        public async Task ViewAsync(CommandContext ctx, DiscordUser user = null)
        {
          DiscordUser victim = user ?? ctx.User;
          var cc = await CurrencyManager.GetAsync(victim.Id);
          var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**{victim.Mention}'s Balance:**\n**`{cc.Balance}🌸 - Enzea`**",
                    Color = DiscordColor.Gray
                };
          await ctx.RespondAsync(embed: embeds);
        }
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddAsync(CommandContext ctx, DiscordUser victim, int Amount)
        {
            var cc = await CurrencyManager.GetAsync(victim.Id);
            var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**{victim.Mention}'s New Balance:**\n**`{cc.Balance + Amount}🌸 - Enzea`**",
                    Color = DiscordColor.Gray
                };
            await CurrencyManager.AddAsync(victim.Id, Amount);
            await ctx.RespondAsync(embed: embeds);
        }
        [Command("take")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task TakeAsync(CommandContext ctx, DiscordUser victim, int Amount)
        {
            var cc = await CurrencyManager.GetAsync(victim.Id);
            cc.Balance -= Amount;
            if(cc.Balance < 0)
                cc.Balance = 0;
            var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**{victim.Mention}'s New Balance:**\n**`{cc.Balance}🌸 - Enzea`**",
                    Color = DiscordColor.Gray
                };
            await CurrencyManager.TakeAsync(victim.Id, Amount);
            await ctx.RespondAsync(embed: embeds);
        }
    }
}
		