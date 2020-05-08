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
    [Group("Admin"), Aliases("a")]
	[Description("Commands for Admins.")]
    [RequirePermissions(Permissions.Administrator)]
	public class AdminModule : BaseCommandModule {
        [GroupCommand]
        public async Task MainCommand(CommandContext ctx)
        {
          await ctx.RespondAsync("You are eligible to use this command.");
        }
        [Group("add")]
        public class AdminModule2 : BaseCommandModule {
            [Command("XP")]
            public async Task AddXpAsync(CommandContext ctx, DiscordUser user, int Amount)
            {
                var cc = await CharManager.GetAsync(user.Id);
                if(cc == null)
                {
                    await ctx.RespondAsync("That person doesn't have a character.");
                    return;
                }
                else
                {
                    await CharManager.XpAsync(user.Id, Amount);
                    await ctx.RespondAsync($"{user.Mention} **gained {Amount} XP from unknown source.**");
                }
            }
            [Command("currency"), Aliases("cur")]
            public async Task AddCurAsync(CommandContext ctx, DiscordUser user, int Amount)
            {
                var cc = await CharManager.GetAsync(user.Id);
                if(cc == null)
                {
                    await ctx.RespondAsync("That person doesn't have a character.");
                    return;
                }
                else
                {
                    await CurrencyManager.AddAsync(user.Id, Amount);
                    await ctx.RespondAsync($"{user.Mention} **gained {Amount}🌸 Enzea from unknown source.**");
                }
            }
            [Command("Statpoints"), Aliases("sp")]
            public async Task AddSPAsync(CommandContext ctx, DiscordUser user, int Amount)
            {
                var cc = await CharManager.GetAsync(user.Id);
                if(cc == null)
                {
                    await ctx.RespondAsync("That person doesn't have a character.");
                    return;
                }
                else
                {
                    await StatsManager.SPAsync(user.Id, Amount);
                    await ctx.RespondAsync($"{user.Mention} **gained {Amount} SP from unknown source.**");
                }
            }
            [Command("Level"), Aliases("lvl")]
            public async Task AddLvlAsync(CommandContext ctx, DiscordUser user, int Amount)
            {
                var cc = await CharManager.GetAsync(user.Id);
                if(cc == null)
                {
                    await ctx.RespondAsync("That person doesn't have a character.");
                    return;
                }
                else
                {
                    await CharManager.CheatLevelAsync(user.Id, Amount);
                    await ctx.RespondAsync($"{user.Mention} **gained {Amount} Level(s) from unknown source.**");
                }
            }
        }
         [Group("take")]
        public class AdminModule3 : BaseCommandModule {
            [Command("currency"), Aliases("cur")]
            public async Task TakeCurAsync(CommandContext ctx, DiscordUser user, int Amount)
            {
                var cc = await CharManager.GetAsync(user.Id);
                if(cc == null)
                {
                    await ctx.RespondAsync("That person doesn't have a character.");
                    return;
                }
                else
                {
                    await CurrencyManager.TakeAsync(user.Id, Amount);
                    await ctx.RespondAsync($"{user.Mention} **lost {Amount}🌸 Enzea out of nowhere.**");
                }
            }
        }
        [Group("reset")]
        public class AdminModule4 : BaseCommandModule {
            [Command("currency"), Aliases("cur")]
            public async Task ResetCurAsync(CommandContext ctx, DiscordUser victim)
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
            [Command("Character"), Aliases("char")]
            public async Task ResetCharAsync(CommandContext ctx, DiscordUser victim)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"{victim.Mention}'s **`character has been wiped.`**",
                    Color = DiscordColor.Gray
                };
                var cc = new Character() {
                    PID = victim.Id,
                    Name = " ",
                    Age = " ",
                    Gender = " ",
                    Level = 1,
                    Xp = 0,
                    Desc = "Nuked Character.",
                    Ref = "https://cdn.discordapp.com/attachments/708249001903783986/708289986360770620/2Q.png"
                };
                var stt = new Stats() {
				PID = ctx.User.Id,
				HP_max = 100,
				HP_current = 100,
				Energy_max = 100,
				Energy_current = 100,
				ATK = 2,
				MELEE = 1,
				RANGED = 1,
				MAGIC = 4,
				SP = 6
			    };
                await CharManager.CheatResetAsync(cc);
                await StatsManager.SyncAsync(stt);
                await ctx.RespondAsync(embed: embeds);
            }
        }
    }
}
		