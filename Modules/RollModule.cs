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
    [Group("roll"), Aliases("r")]
	[Description("The Roll Part Of This Bot")]
	public class RollModule : BaseCommandModule {
        private Random _rng = new Random();
        [GroupCommand]
        public async Task NormRollAsync(CommandContext ctx, [RemainingText, Description("Your action")] String Question)
        {
          int r = _rng.Next(12);
           var embeds = new DiscordEmbedBuilder
                {
                    Title = Question,
                    Description = "**You rolled** **`" + r + "`**",
                    Color = DiscordColor.Gray
                };
          await ctx.RespondAsync(embed : embeds);
        }
        [Command("attack"), Aliases("atk")]
        public async Task AtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var Acc = await CharManager.GetAsync(ctx.User.Id);
            if(Acc == null)
            {
                await ctx.RespondAsync("You don't have a character to attack with.");
                return;
            }
            var Tcc = await CharManager.GetAsync(user.Id);
            if(Tcc == null)
            {
                await ctx.RespondAsync("Target has no character to attack.");
                return;
            }
            var Asc = await StatsManager.GetAsync(ctx.User.Id);
            var Tsc = await StatsManager.GetAsync(user.Id);
            if(Asc.HP_current < 1)
            {
                await ctx.RespondAsync("You are dead... You cannot attack");
                return;
            }
            if(Asc.Energy_current < 4)
            {
                await ctx.RespondAsync("You have no energy to attack");
                return;
            }
            int Ar = _rng.Next(12) + 1;
            int Tr = _rng.Next(12) + 1;
            var Ar1 = Ar + Asc.MELEE;
            var Tr1 = Tr + Tsc.MELEE;
            if(Ar == 1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {Tsc.MELEE}`** and failed critically",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else if(Ar1 > Tr1) 
            {
                Tsc.HP_current -= Asc.ATK;
                var damage = Asc.ATK;
                if(Ar == 12)
                {
                    damage += 4;
                    Tsc.HP_current -= 4;
                }
                if(Tsc.HP_current < 0)
                    Tsc.HP_current = 0;
                Asc.Energy_current -= _rng.Next(5);
                await StatsManager.SyncAsync(Tsc);
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {Tsc.MELEE}`** and succeeded.",
                    Color = DiscordColor.Gray
                };
                embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                await ctx.RespondAsync(embed: embeds);
            }
            else if( Ar1 == Tr1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {Tsc.MELEE}`** and failed by 1 dice side...",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {Tsc.MELEE}`** and failed",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
        }
        [Command("ranged"), Aliases("ratk")]
        public async Task RAtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var Acc = await CharManager.GetAsync(ctx.User.Id);
            if(Acc == null)
            {
                await ctx.RespondAsync("You don't have a character to attack with.");
                return;
            }
            var Tcc = await CharManager.GetAsync(user.Id);
            if(Tcc == null)
            {
                await ctx.RespondAsync("Target has no character to attack.");
                return;
            }
            var Asc = await StatsManager.GetAsync(ctx.User.Id);
            var Tsc = await StatsManager.GetAsync(user.Id);
            if(Asc.HP_current < 1)
            {
                await ctx.RespondAsync("You are dead... You cannot attack");
                return;
            }
            if(Asc.Energy_current < 6)
            {
                await ctx.RespondAsync("You have no energy to attack from range");
                return;
            }
            int Ar = _rng.Next(12) + 1;
            int Tr = _rng.Next(12) + 1;
            var Ar1 = Ar + Asc.RANGED;
            var Tr1 = Tr + Tsc.RANGED;
            if(Ar == 1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed critically",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else if(Ar1 > Tr1) 
            {
                Tsc.HP_current -= Asc.ATK/2;
                var damage = Asc.ATK/2;
                if(Ar == 12)
                {
                    damage += 6;
                    Tsc.HP_current -= 6;
                }
                if(Tsc.HP_current < 0)
                    Tsc.HP_current = 0;
                Asc.Energy_current -= _rng.Next(7);
                await StatsManager.SyncAsync(Tsc);
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and succeeded.",
                    Color = DiscordColor.Gray
                };
                embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                await ctx.RespondAsync(embed: embeds);
            }
            else if( Ar1 == Tr1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed by 1 dice side...",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
        }
        [Command("magic"), Aliases("mag")]
        public async Task MAtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var Acc = await CharManager.GetAsync(ctx.User.Id);
            if(Acc == null)
            {
                await ctx.RespondAsync("You don't have a character to attack with.");
                return;
            }
            var Tcc = await CharManager.GetAsync(user.Id);
            if(Tcc == null)
            {
                await ctx.RespondAsync("Target has no character to attack.");
                return;
            }
            var Asc = await StatsManager.GetAsync(ctx.User.Id);
            var Tsc = await StatsManager.GetAsync(user.Id);
            if(Asc.HP_current < 1)
            {
                await ctx.RespondAsync("You are dead... You cannot attack");
                return;
            }
            if(Asc.Energy_current < 20)
            {
                await ctx.RespondAsync("You have no energy to cast magic.");
                return;
            }
            int Ar = _rng.Next(12) + 1;
            int Tr = _rng.Next(12) + 1;
            var Ar1 = Ar + Asc.MAGIC;
            var Tr1 = Tr + Tsc.MAGIC;
            if(Ar == 1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Casts an offensive spell towards **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed critically",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else if(Ar1 > Tr1) 
            {
                Tsc.HP_current -= Asc.MAGIC;
                var damage = Asc.MAGIC;
                if(Ar == 12)
                {
                    damage += 8;
                    Tsc.HP_current -= 8;
                }
                if(Tsc.HP_current < 0)
                    Tsc.HP_current = 0;
                Asc.Energy_current -= 20;
                await StatsManager.SyncAsync(Asc);
                await StatsManager.SyncAsync(Tsc);
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Casts an offensive spell towards **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and succeeded.",
                    Color = DiscordColor.Gray
                };
                embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                await ctx.RespondAsync(embed: embeds);
            }
            else if( Ar1 == Tr1)
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Casts an offensive spell towards **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed by 1 dice side...",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
            else
            {
                var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**`{Acc.Name}`** Casts an offensive spell towards **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed",
                    Color = DiscordColor.Gray
                };
                await ctx.RespondAsync(embed: embeds);
            }
        }
    }
}
		