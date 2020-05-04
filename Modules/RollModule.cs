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
        [Cooldown(2, 10, CooldownBucketType.Channel)]
        public async Task AtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var prox = await NPCManager.ProxyGetAsync(ctx.User.Id);
            if(prox == null) 
            {
                prox = await NPCManager.ProxyGetAsync(user.Id);
                if(prox == null) 
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
                else
                {
                    var npc = await NPCManager.GetAsync(prox.NPCID);
                    var Acc = await CharManager.GetAsync(ctx.User.Id);
                    if(Acc == null)
                    {
                        await ctx.RespondAsync("You don't have a character to attack with.");
                        return;
                    }
                    var Asc = await StatsManager.GetAsync(ctx.User.Id);
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
                    var Tr1 = Tr + npc.MBonus;
                    if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {npc.MBonus}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else if(Ar1 > Tr1) 
                    {
                        npc.HP_current -= Asc.ATK;
                        var damage = Asc.ATK;
                        if(Ar == 12)
                        {
                            damage += 4;
                            npc.HP_current -= 4;
                        }
                        if(npc.HP_current < 0)
                            npc.HP_current = 0;
                        Asc.Energy_current -= _rng.Next(5);
                        await NPCManager.SyncAsync(npc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {npc.MBonus}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MELEE}`** against **`{Tr} + {npc.MBonus}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                }
            }
            else
            {
                var npc = await NPCManager.GetAsync(prox.NPCID);
                var Tcc = await CharManager.GetAsync(user.Id);
                if(Tcc == null)
                    {
                        await ctx.RespondAsync("Target has no character to attack.");
                        return;
                    }
                var Tsc = await StatsManager.GetAsync(user.Id);
                int Ar = _rng.Next(12) + 1;
                int Tr = _rng.Next(12) + 1;
                var Ar1 = Ar + npc.MBonus;
                var Tr1 = Tr + Tsc.MELEE;
                if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MBonus}`** against **`{Tr} + {Tsc.MELEE}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                else if(Ar1 > Tr1) 
                    {
                        Tsc.HP_current -= npc.ATK;
                        var damage = npc.ATK;
                        if(Ar == 12)
                        {
                            damage += 4;
                            Tsc.HP_current -= 4;
                        }
                        if(Tsc.HP_current < 0)
                            Tsc.HP_current = 0;
                        await StatsManager.SyncAsync(Tsc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MBonus}`** against **`{Tr} + {Tsc.MELEE}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MBonus}`** against **`{Tr} + {Tsc.MELEE}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
            }
            
        }
        [Command("ranged"), Aliases("ratk")]
        [Cooldown(2, 10, CooldownBucketType.Channel)]
        public async Task RAtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var prox = await NPCManager.ProxyGetAsync(ctx.User.Id);
            if(prox == null) 
            {
                prox = await NPCManager.ProxyGetAsync(user.Id);
                if(prox == null) 
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
                    if(Asc.Energy_current < 10)
                    {
                        await ctx.RespondAsync("You have no energy to attack");
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
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed critically",
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
                            damage += 8;
                            Tsc.HP_current -= 8;
                        }
                        if(Tsc.HP_current < 0)
                            Tsc.HP_current = 0;
                        Asc.Energy_current -= _rng.Next(11);
                        await StatsManager.SyncAsync(Tsc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else if( Ar1 == Tr1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed by 1 dice side...",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {Tsc.RANGED}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                }
                else
                {
                    var npc = await NPCManager.GetAsync(prox.NPCID);
                    var Acc = await CharManager.GetAsync(ctx.User.Id);
                    if(Acc == null)
                    {
                        await ctx.RespondAsync("You don't have a character to attack with.");
                        return;
                    }
                    var Asc = await StatsManager.GetAsync(ctx.User.Id);
                    if(Asc.HP_current < 1)
                    {
                        await ctx.RespondAsync("You are dead... You cannot attack");
                        return;
                    }
                    if(Asc.Energy_current < 10)
                    {
                        await ctx.RespondAsync("You have no energy to attack");
                        return;
                    }
                    int Ar = _rng.Next(12) + 1;
                    int Tr = _rng.Next(12) + 1;
                    var Ar1 = Ar + Asc.RANGED;
                    var Tr1 = Tr + npc.RBonus;
                    if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {npc.RBonus}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else if(Ar1 > Tr1) 
                    {
                        npc.HP_current -= Asc.ATK/2;
                        var damage = Asc.ATK/2;
                        if(Ar == 12)
                        {
                            damage += 8;
                            npc.HP_current -= 8;
                        }
                        if(npc.HP_current < 0)
                            npc.HP_current = 0;
                        Asc.Energy_current -= _rng.Next(11);
                        await NPCManager.SyncAsync(npc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {npc.RBonus}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Attacks from a distance **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.RANGED}`** against **`{Tr} + {npc.RBonus}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                }
            }
            else
            {
                var npc = await NPCManager.GetAsync(prox.NPCID);
                var Tcc = await CharManager.GetAsync(user.Id);
                if(Tcc == null)
                    {
                        await ctx.RespondAsync("Target has no character to attack.");
                        return;
                    }
                var Tsc = await StatsManager.GetAsync(user.Id);
                int Ar = _rng.Next(12) + 1;
                int Tr = _rng.Next(12) + 1;
                var Ar1 = Ar + npc.RBonus;
                var Tr1 = Tr + Tsc.RANGED;
                if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.RBonus}`** against **`{Tr} + {Tsc.RANGED}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                else if(Ar1 > Tr1) 
                    {
                        Tsc.HP_current -= npc.ATK/2;
                        var damage = npc.ATK/2;
                        if(Ar == 12)
                        {
                            damage += 8;
                            Tsc.HP_current -= 8;
                        }
                        if(Tsc.HP_current < 0)
                            Tsc.HP_current = 0;
                        await StatsManager.SyncAsync(Tsc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.RBonus}`** against **`{Tr} + {Tsc.RANGED}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Attacks from a distance **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.RBonus}`** against **`{Tr} + {Tsc.RANGED}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
            }
        }
        [Command("magic"), Aliases("mag")]
        [Cooldown(2, 10, CooldownBucketType.Channel)]
        public async Task MAtkRollAsync(CommandContext ctx, DiscordUser user)
        {
            var prox = await NPCManager.ProxyGetAsync(ctx.User.Id);
            if(prox == null) 
            {
                prox = await NPCManager.ProxyGetAsync(user.Id);
                if(prox == null) 
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
                        await ctx.RespondAsync("You have no energy to attack");
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
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed critically",
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
                            damage += 12;
                            Tsc.HP_current -= 12;
                        }
                        if(Tsc.HP_current < 0)
                            Tsc.HP_current = 0;
                        Asc.Energy_current -= _rng.Next(21);
                        await StatsManager.SyncAsync(Tsc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell one **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else if( Ar1 == Tr1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed by 1 dice side...",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {Tsc.MAGIC}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                }
                else
                {
                    var npc = await NPCManager.GetAsync(prox.NPCID);
                    var Acc = await CharManager.GetAsync(ctx.User.Id);
                    if(Acc == null)
                    {
                        await ctx.RespondAsync("You don't have a character to attack with.");
                        return;
                    }
                    var Asc = await StatsManager.GetAsync(ctx.User.Id);
                    if(Asc.HP_current < 1)
                    {
                        await ctx.RespondAsync("You are dead... You cannot attack");
                        return;
                    }
                    if(Asc.Energy_current < 20)
                    {
                        await ctx.RespondAsync("You have no energy to attack");
                        return;
                    }
                    int Ar = _rng.Next(12) + 1;
                    int Tr = _rng.Next(12) + 1;
                    var Ar1 = Ar + Asc.MAGIC;
                    var Tr1 = Tr + npc.MagBonus;
                    if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {npc.MagBonus}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else if(Ar1 > Tr1) 
                    {
                        npc.HP_current -= Asc.MAGIC;
                        var damage = Asc.MAGIC;
                        if(Ar == 12)
                        {
                            damage += 12;
                            npc.HP_current -= 12;
                        }
                        if(npc.HP_current < 0)
                            npc.HP_current = 0;
                        Asc.Energy_current -= _rng.Next(21);
                        await NPCManager.SyncAsync(npc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {npc.MagBonus}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{Acc.Name}`** `{Asc.HP_current}`**/**`{Asc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                    else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{Acc.Name}`** Casts an offensive spell on **`{npc.Name}`**\n\n**`{Acc.Name}`** rolled **`{Ar} + {Asc.MAGIC}`** against **`{Tr} + {npc.MagBonus}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                }
            }
            else
            {
                var npc = await NPCManager.GetAsync(prox.NPCID);
                var Tcc = await CharManager.GetAsync(user.Id);
                if(Tcc == null)
                    {
                        await ctx.RespondAsync("Target has no character to attack.");
                        return;
                    }
                var Tsc = await StatsManager.GetAsync(user.Id);
                int Ar = _rng.Next(12) + 1;
                int Tr = _rng.Next(12) + 1;
                var Ar1 = Ar + npc.MagBonus;
                var Tr1 = Tr + Tsc.RANGED;
                if(Ar == 1)
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MagBonus}`** against **`{Tr} + {Tsc.MAGIC}`** and failed critically",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
                else if(Ar1 > Tr1) 
                    {
                        Tsc.HP_current -= npc.ATK/2;
                        var damage = npc.ATK/2;
                        if(Ar == 12)
                        {
                            damage += 12;
                            Tsc.HP_current -= 12;
                        }
                        if(Tsc.HP_current < 0)
                            Tsc.HP_current = 0;
                        await StatsManager.SyncAsync(Tsc);
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MagBonus}`** against **`{Tr} + {Tsc.MAGIC}`** and succeeded.",
                            Color = DiscordColor.Gray
                        };
                        embeds = embeds.AddField("Info", $"**`{npc.Name}`** `{npc.HP_current}`**/**`{npc.HP_max}` **HP**\n**Inflicted {damage} damage**\n**`{Tcc.Name}`** `{Tsc.HP_current}`**/**`{Tsc.HP_max}` **HP**");
                        await ctx.RespondAsync(embed: embeds);
                    }
                else
                    {
                        var embeds = new DiscordEmbedBuilder
                        {
                            Description = $"**`{npc.Name}`** Casts an offensive spell on **`{Tcc.Name}`**\n\n**`{npc.Name}`** rolled **`{Ar} + {npc.MagBonus}`** against **`{Tr} + {Tsc.MAGIC}`** and failed",
                            Color = DiscordColor.Gray
                        };
                        await ctx.RespondAsync(embed: embeds);
                    }
            }
        }
    }
}
		