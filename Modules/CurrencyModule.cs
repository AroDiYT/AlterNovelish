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
            embeds = embeds.WithFooter($"Added {Amount}🌸");
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
            embeds = embeds.WithFooter($"Took {Amount}🌸");
            await CurrencyManager.TakeAsync(victim.Id, Amount);
            await ctx.RespondAsync(embed: embeds);
        }
        [Command("fix")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task FixAsync(CommandContext ctx, DiscordUser victim)
        {
            var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**{victim.Mention}'s Balance:**\n**`100🌸 - Enzea`**",
                    Color = DiscordColor.Gray
                };
            var cc = new Currency() {
                PID = victim.Id,
                Balance = 100
            };
            await CurrencyManager.SyncAsync(cc);
            await ctx.RespondAsync(embed: embeds);
        }
        [Command("work")]
        [Aliases("w")]
        [Cooldown(1,120,CooldownBucketType.User)]
        public async Task WorkAsync(CommandContext ctx)
        {
            var cc = await CurrencyManager.GetAsync(ctx.User.Id);
            var ob = cc.Balance;
            cc.Balance += _rng.Next(101);
            var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**{ctx.User.Mention} worked for** **`{cc.Balance - ob}🌸 - Enzea`**",
                    Color = DiscordColor.Gray
                };
            embeds = embeds.WithFooter($"Added {cc.Balance - ob}🌸");
            await CurrencyManager.SyncAsync(cc);
            await ctx.RespondAsync(embed: embeds);
        }
        [Command("Transfer")]
        public async Task TransferAsync(CommandContext ctx, DiscordUser user, int Amount)
        {
            var ACur = await CurrencyManager.GetAsync(ctx.User.Id);
            if(ACur.Balance < Amount)
            {
                await ctx.RespondAsync("You don't have that much Enzea's");
                return;
            }
            var TCur = await CurrencyManager.GetAsync(user.Id);
            TCur.Balance += Amount;
            ACur.Balance -= Amount;
            await CurrencyManager.SyncAsync(TCur);
            await CurrencyManager.SyncAsync(ACur);
            var embeds = new DiscordEmbedBuilder
                {
                    Description = $"{ctx.User.Mention} **Transfers** `{Amount}🌸` **Enzea's over to** {user.Mention}",
                    Color = DiscordColor.Gray
                };
            await ctx.RespondAsync(embed: embeds);
        }
    }
}
		