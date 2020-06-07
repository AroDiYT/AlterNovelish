using System;
using System.Threading.Tasks;
using System.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;
using BotTemplate.Helpers;

namespace BotTemplate.Modules
{
    [Group("Fight")]
    public class Fight : BaseCommandModule
    {
        [Group("roll"), Description("Roll with your stats as modifiers.")]
        public class Rolls : BaseCommandModule
        {
            [GroupCommand]
            public async Task RollAsync(CommandContext ctx, int max)
            {
                if (max < 2)
                    max = 12;
                var Roll = Helpers.Tools.RNG.Next(max) + 1;
                await ctx.RespondAsync("**You rolled a " + Roll + "**");
            }
            [GroupCommand]
            public async Task RollAsync(CommandContext ctx)
            {
                var max = 12;
                var Roll = Helpers.Tools.RNG.Next(max) + 1;
                await ctx.RespondAsync("**You rolled a " + Roll + "**");
            }
            [Command("Thought"), Aliases("mind")]
            public async Task ThoughtAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);

                var RollN = Helpers.Tools.RNG.Next(12) + 1;
                var Bonus = Chr.Thought;

                var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus / 2;
                var emmys = new DiscordEmbedBuilder();
                emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (thought){Bonus}\nvs\nTarget: {TargetR}");
                if (RollN + Bonus > TargetR)
                {
                    emmys = emmys.WithDescription("You've succeeded.");
                }
                else
                {
                    emmys = emmys.WithDescription("You've failed.");
                }
                await ctx.RespondAsync(embed: emmys);
            }
            [Command("Speed"), Aliases("spd")]
            public async Task SpeedAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);

                var RollN = Helpers.Tools.RNG.Next(12) + 1;
                var Bonus = Chr.Speed;

                var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus / 2;
                var emmys = new DiscordEmbedBuilder();
                emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (speed){Bonus}\nvs\nTarget: {TargetR}");
                if (RollN + Bonus > TargetR)
                {
                    emmys = emmys.WithDescription("You've succeeded.");
                }
                else
                {
                    emmys = emmys.WithDescription("You've failed.");
                }
                await ctx.RespondAsync(embed: emmys);
            }
            [Command("Strenght"), Aliases("str")]
            public async Task StrAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);

                var RollN = Helpers.Tools.RNG.Next(12) + 1;
                var Bonus = Chr.Strenght;

                var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus / 2;
                var emmys = new DiscordEmbedBuilder();
                emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (strenght){Bonus}\nvs\nTarget: {TargetR}");
                if (RollN + Bonus > TargetR)
                {
                    emmys = emmys.WithDescription("You've succeeded.");
                }
                else
                {
                    emmys = emmys.WithDescription("You've failed.");
                }
                await ctx.RespondAsync(embed: emmys);
            }
            [Command("intellegence"), Aliases("int")]
            public async Task intAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);

                var RollN = Helpers.Tools.RNG.Next(12) + 1;
                var Bonus = Chr.Intellegence;

                var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus / 2;
                var emmys = new DiscordEmbedBuilder();
                emmys = emmys.WithAuthor($"{Chr.Name} rolls\n {RollN} + (Intellegence){Bonus}\nvs\nTarget:\n {TargetR}");
                if (RollN + Bonus > TargetR)
                {
                    emmys = emmys.WithDescription("Yes, you figured out how that worked.");
                }
                else
                {
                    emmys = emmys.WithDescription("Seems your intellegence failed you...");
                }
                await ctx.RespondAsync(embed: emmys);
            }
            [Command("Dodge")]
            public async Task DodgeAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);

                var RollN = Helpers.Tools.RNG.Next(12) + 1;
                var Bonus = Chr.Dodge;

                var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus / 2;
                var emmys = new DiscordEmbedBuilder();
                emmys = emmys.WithAuthor($"{Chr.Name} rolls\n {RollN} + (Dodge){Bonus}\nvs\nTarget:\n {TargetR}");
                if (RollN + Bonus > TargetR)
                {
                    emmys = emmys.WithDescription("You dodged it.");
                }
                else
                {
                    emmys = emmys.WithDescription("You failed to dodge it.");
                }
                await ctx.RespondAsync(embed: emmys);
            }
        }
        [Command("Melee"), Aliases("Atk")]
        [Description("Attack from melee range.")]
        public async Task AttackAsync(CommandContext ctx, [Description("Target")] DiscordUser target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Accg2 = new AccountGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id
            };
            var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
            if (Check2 == null)
            {
                await ctx.RespondAsync("Target has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }

            var OwnGet2 = new OwnerGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id,
                Slot = Check2.Slot
            };
            var Own2 = await ManageCharacter.GetAll(OwnGet2);
            var ChrGet2 = new ChrGet()
            {
                Entry = Own2.CharEntry,
            };
            var Chr2 = await ManageCharacter.GetAll(ChrGet2);
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Melee);
            await ctx.RespondAsync(embed: emm);
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 5);
        }
        [Command("Melee")]
        public async Task AttackAsync(CommandContext ctx, [RemainingText, Description("Target")] string target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Chr2 = await ManageCharacter.GetChrByName(target);
            if (Chr2 == null)
            {
                await ctx.RespondAsync("Does not exist.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Melee);
            await ctx.RespondAsync(embed: emm);
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 5);
        }
        [Command("Ranged")]
        [Description("Attack from a distance.")]
        public async Task RAttackAsync(CommandContext ctx, [Description("Target")] DiscordUser target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Accg2 = new AccountGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id
            };
            var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
            if (Check2 == null)
            {
                await ctx.RespondAsync("Target has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 5);
            var OwnGet2 = new OwnerGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id,
                Slot = Check2.Slot
            };
            var Own2 = await ManageCharacter.GetAll(OwnGet2);
            var ChrGet2 = new ChrGet()
            {
                Entry = Own2.CharEntry,
            };
            var Chr2 = await ManageCharacter.GetAll(ChrGet2);
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Ranged);
            await ctx.RespondAsync(embed: emm);
        }
        [Command("Ranged")]
        public async Task RAttackAsync(CommandContext ctx, [RemainingText, Description("Target")] string target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Chr2 = await ManageCharacter.GetChrByName(target);
            if (Chr2 == null)
            {
                await ctx.RespondAsync("Does not exist.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 5);
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Ranged);
            await ctx.RespondAsync(embed: emm);
        }
        [Command("Magic"), Aliases("Matk")]
        [Description("Attack with magic from an distance")]
        public async Task MagicAsync(CommandContext ctx, [Description("Target")] DiscordUser target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Accg2 = new AccountGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id
            };
            var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
            if (Check2 == null)
            {
                await ctx.RespondAsync("Target has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 10);

            var OwnGet2 = new OwnerGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id,
                Slot = Check2.Slot
            };
            var Own2 = await ManageCharacter.GetAll(OwnGet2);
            var ChrGet2 = new ChrGet()
            {
                Entry = Own2.CharEntry,
            };
            var Chr2 = await ManageCharacter.GetAll(ChrGet2);
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Magic);
            await ctx.RespondAsync(embed: emm);
        }
        [Command("Magic")]
        public async Task MagicAsync(CommandContext ctx, [RemainingText, Description("Target")] string target)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var Chr2 = await ManageCharacter.GetChrByName(target);
            if (Chr2 == null)
            {
                await ctx.RespondAsync("Does not exist.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 10);
            var emm = await Helpers.Attack.RollAsync(Chr, Chr2, FightOptions.Magic);
            await ctx.RespondAsync(embed: emm);
        }
        [Command("Heal")]
        [Description("Heal a characters.")]
        public async Task HealAsync(CommandContext ctx, [Description("Target")] DiscordUser target = null)
        {
            target = target ?? ctx.User;
            var Accg = new AccountGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("Target has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            if (Chr.HPC > 0 && await Helpers.Owners.CheckAsync(ctx.User.Id))
            {
                Chr.HPC = Chr.HPM;
                Chr.ENC = Chr.ENM;
                await ManageCharacter.UpdateAsync(Chr);
                await ctx.RespondAsync($"**{Chr.Name} has been healed**");
                return;
            }
            else if (Chr.HPC < 1)
            {
                Chr.HPC = Chr.HPM;
                Chr.ENC = Chr.ENM;
                await ManageCharacter.UpdateAsync(Chr);
                await ctx.RespondAsync($"**{Chr.Name} has been healed**");
                return;
            }
            else
            {
                await ctx.RespondAsync("You can't use this if you aren't dead. Ask an admin for help if needed.");
            }
        }
        [Command("Heal")]
        public async Task HealAsync(CommandContext ctx, [RemainingText, Description("Target")] string target)
        {
            var Chr = await ManageCharacter.GetChrByName(target);
            if (Chr == null)
            {
                await ctx.RespondAsync("Does not exist.");
                return;
            }
            if (Chr.HPC > 0 && await Helpers.Owners.CheckAsync(ctx.User.Id))
            {
                Chr.HPC = Chr.HPM;
                Chr.ENC = Chr.ENM;
                await ManageCharacter.UpdateAsync(Chr);
                await ctx.RespondAsync($"**{Chr.Name} has been healed**");
                return;
            }
            else if (Chr.HPC < 1 && await Helpers.Owners.CheckAsync(ctx.User.Id))
            {
                Chr.HPC = Chr.HPM;
                Chr.ENC = Chr.ENM;
                await ManageCharacter.UpdateAsync(Chr);
                await ctx.RespondAsync($"**{Chr.Name} has been healed**");
                return;
            }
            else
            {
                await ctx.RespondAsync("You can't use this cmd");
            }
        }
        [Command("Rest")]
        public async Task HealAsync(CommandContext ctx)
        {
            var target = ctx.User;
            var Accg = new AccountGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = target.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
            if (Cdr != null)
            {
                await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 1200);
            Chr.HPC = Chr.HPM;
            Chr.ENC = Chr.ENM;
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync($"**{Chr.Name} has been healed**");
            return;
        }
        [Group("Special"), Aliases("s")]
        public class Specials : BaseCommandModule
        {
            //Priest
            [Command("Bless"),Description("Only for Priests")]
            public async Task Bless(CommandContext ctx, DiscordUser Target = null)
            {
                var target = ctx.User;
                var Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ctx.Command.Name);
                if (Cdr != null)
                {
                    await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                    return;
                }
                if(Chr.SubClass != SubClasses.Priest)
                {
                    await ctx.RespondAsync("You are not a priest");
                    return;
                }
                target = Target ?? ctx.User;
                Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("no characters.");
                    return;
                }
                OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                Own = await ManageCharacter.GetAll(OwnGet);
                ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var TChr = await ManageCharacter.GetAll(ChrGet);
                var Amount = Chr.SubClassAttribute + Chr.MagicEff;
                await Helpers.Cast.Heal(Amount, TChr);
                await ctx.RespondAsync($"`{Chr.Name}` blesses `{TChr.Name}`, as they heal for `{Amount} HP`");
                await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 900);
            }
            [Command("Bless")]
            public async Task Bless(CommandContext ctx, [RemainingText] string Target)
            {
                var target = ctx.User;
                var Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if(Chr.SubClass != SubClasses.Priest)
                {
                    await ctx.RespondAsync("You are not a priest");
                    return;
                }
                var TChr = await ManageCharacter.GetChrByName(Target);
                if (TChr == null)
                {
                    await ctx.RespondAsync("Does not exist.");
                    return;
                }
                var Amount = Chr.SubClassAttribute + Chr.MagicEff;
                await Helpers.Cast.Heal(Amount, TChr);
                await ctx.RespondAsync($"`{Chr.Name}` blesses `{TChr.Name}`, as they heal for `{Amount} HP`");
                await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 900);
            }
            //Wizard
            [Command("EnergyBall")]
            public async Task EnergyBall(CommandContext ctx, DiscordUser Target = null)
            {
                var target = ctx.User;
                var Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if(Chr.SubClass != SubClasses.Wizard)
                {
                    await ctx.RespondAsync("You are not a Wizard");
                    return;
                }
                target = Target ?? ctx.User;
                Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("no characters.");
                    return;
                }
                OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                Own = await ManageCharacter.GetAll(OwnGet);
                ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var TChr = await ManageCharacter.GetAll(ChrGet);
                var Amount = Chr.SubClassAttribute + Chr.MagicEff;
                await Helpers.Cast.Damage(Amount*2, TChr);
                await ctx.RespondAsync($"`{Chr.Name}` fires an energy ball at `{TChr.Name}`, as they burn for `{Amount*2} HP`");
                await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 1900);
            }
            [Command("EnergyBall")]
            public async Task EnergyBall(CommandContext ctx, [RemainingText] string Target)
            {
                var target = ctx.User;
                var Accg = new AccountGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = target.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if(Chr.SubClass != SubClasses.Wizard)
                {
                    await ctx.RespondAsync("You are not a Wizard");
                    return;
                }
                var TChr = await ManageCharacter.GetChrByName(Target);
                if (TChr == null)
                {
                    await ctx.RespondAsync("Does not exist.");
                    return;
                }
                var Amount = Chr.SubClassAttribute + Chr.MagicEff;
                await Helpers.Cast.Damage(Amount*2, TChr);
                await ctx.RespondAsync($"`{Chr.Name}` fires an energy ball at `{TChr.Name}`, as they burn for `{Amount*2} HP`");
                await Helpers.TimerChr.AddCooldown(Chr.Entry, ctx.Command.Name, 1900);
            }
        }
    }
}
