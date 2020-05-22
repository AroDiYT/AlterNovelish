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

namespace BotTemplate.Modules {
  [Group("Fight")]
	public class Fight : BaseCommandModule {
   [Group("roll")]
   public class Rolls : BaseCommandModule {
     [GroupCommand]
     public async Task RollAsync(CommandContext ctx, int max)
     {
       if(max < 2)
        max = 12;
       var Roll = Helpers.Tools.RNG.Next(max) + 1;
       await ctx.RespondAsync("**You rolled a " + Roll + "**");
     }
      [Command("Thought"), Aliases("mind")]
    public async Task ThoughtAsync(CommandContext ctx)
    {
        var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var RollN = Helpers.Tools.RNG.Next(12) + 1;
          var Bonus = Chr.Thought;

          var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus/2;
          var emmys = new DiscordEmbedBuilder();
          emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (thought){Bonus}\nvs\nTarget: {TargetR}");
          if(RollN + Bonus > TargetR)
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
        var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var RollN = Helpers.Tools.RNG.Next(12) + 1;
          var Bonus = Chr.Speed;

          var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus/2;
          var emmys = new DiscordEmbedBuilder();
          emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (speed){Bonus}\nvs\nTarget: {TargetR}");
          if(RollN + Bonus > TargetR)
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
        var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var RollN = Helpers.Tools.RNG.Next(12) + 1;
          var Bonus = Chr.Strenght;

          var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus/2;
          var emmys = new DiscordEmbedBuilder();
          emmys = emmys.WithAuthor($"{Chr.Name} rolls {RollN} + (strenght){Bonus}\nvs\nTarget: {TargetR}");
          if(RollN + Bonus > TargetR)
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
        var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var RollN = Helpers.Tools.RNG.Next(12) + 1;
          var Bonus = Chr.Intellegence;

          var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus/2;
          var emmys = new DiscordEmbedBuilder();
          emmys = emmys.WithAuthor($"{Chr.Name} rolls\n {RollN} + (Intellegence){Bonus}\nvs\nTarget:\n {TargetR}");
          if(RollN + Bonus > TargetR)
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
        var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var RollN = Helpers.Tools.RNG.Next(12) + 1;
          var Bonus = Chr.Dodge;

          var TargetR = Helpers.Tools.RNG.Next(12) + 1 + Bonus/2;
          var emmys = new DiscordEmbedBuilder();
          emmys = emmys.WithAuthor($"{Chr.Name} rolls\n {RollN} + (Dodge){Bonus}\nvs\nTarget:\n {TargetR}");
          if(RollN + Bonus > TargetR)
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
      [Command("Melee"),Aliases("Atk")]
        public async Task AttackAsync(CommandContext ctx, DiscordUser target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Accg2 = new AccountGet() {
            UID = target.Id,
            GID = ctx.Guild.Id
          };
          var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
          if(Check2 == null)
          {
            await ctx.RespondAsync("Target has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var OwnGet2 = new OwnerGet() {
            UID = target.Id,
            GID = ctx.Guild.Id,
            Slot = Check2.Slot
          };
          var Own2 = await ManageCharacter.GetAll(OwnGet2);
          var ChrGet2 = new ChrGet() {
            Entry = Own2.CharEntry,
          };
          var Chr2 = await ManageCharacter.GetAll(ChrGet2);
          var Mod = new Modifiers() {
            Attack = Chr.Sleight,
            Defend = Chr2.Dodge,
            DamageBonus = 4
          };
          var emm = await Helpers.Attack.RollAsync(Mod, ctx.User.Id, target.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Melee")]
        public async Task AttackAsync(CommandContext ctx, [RemainingText] string target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Chr2 = await ManageCharacter.GetChrByName(target);
          if(Chr2 == null)
          {
            await ctx.RespondAsync("Does not exist.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          var Mod = new Modifiers() {
            Attack = Chr.Sleight,
            Defend = Chr2.Dodge,
            DamageBonus = 4
          };
          var emm = await Helpers.Attack.RollAsync(Mod, ctx.User.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Ranged")]
        public async Task RAttackAsync(CommandContext ctx, DiscordUser target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Accg2 = new AccountGet() {
            UID = target.Id,
            GID = ctx.Guild.Id
          };
          var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
          if(Check2 == null)
          {
            await ctx.RespondAsync("Target has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var OwnGet2 = new OwnerGet() {
            UID = target.Id,
            GID = ctx.Guild.Id,
            Slot = Check2.Slot
          };
          var Own2 = await ManageCharacter.GetAll(OwnGet2);
          var ChrGet2 = new ChrGet() {
            Entry = Own2.CharEntry,
          };
          var Chr2 = await ManageCharacter.GetAll(ChrGet2);
          var Mod = new Modifiers() {
            Attack = Chr.Marksman,
            Defend = Chr2.Dodge
          };
          var emm = await Helpers.Attack.RollAsync(Mod, ctx.User.Id, target.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Magic")]
        public async Task MAttackAsync(CommandContext ctx, DiscordUser target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Accg2 = new AccountGet() {
            UID = target.Id,
            GID = ctx.Guild.Id
          };
          var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
          if(Check2 == null)
          {
            await ctx.RespondAsync("Target has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);

          var OwnGet2 = new OwnerGet() {
            UID = target.Id,
            GID = ctx.Guild.Id,
            Slot = Check2.Slot
          };
          var Own2 = await ManageCharacter.GetAll(OwnGet2);
          var ChrGet2 = new ChrGet() {
            Entry = Own2.CharEntry,
          };
          var Chr2 = await ManageCharacter.GetAll(ChrGet2);
          var Mod = new Modifiers() {
            Attack = Chr.Magic,
            Defend = Chr2.Magic
          };
           if(Chr.ENC < 20)
          {
            await ctx.RespondAsync("You have no energy!");
            return;
          }
          Chr.ENC -= 20;
          if(Chr.ENC < 1) {
            Chr.ENC = 0;
          }
          var emm = await Helpers.Attack.MagicRollAsync(Mod, ctx.User.Id, target.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Ranged")]
        public async Task RAttackAsync(CommandContext ctx, [RemainingText] string target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Chr2 = await ManageCharacter.GetChrByName(target);
          if(Chr2 == null)
          {
            await ctx.RespondAsync("Does not exist.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          var Mod = new Modifiers() {
            Attack = Chr.Marksman,
            Defend = Chr2.Dodge
          };
          var emm = await Helpers.Attack.RollAsync(Mod, ctx.User.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Magic")]
        public async Task MAttackAsync(CommandContext ctx, [RemainingText] string target)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var Chr2 = await ManageCharacter.GetChrByName(target);
          if(Chr2 == null)
          {
            await ctx.RespondAsync("Does not exist.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          var Mod = new Modifiers() {
            Attack = Chr.Magic,
            Defend = Chr2.Magic
          };
           if(Chr.ENC < 20)
          {
            await ctx.RespondAsync("You have no energy!");
            return;
          }
          Chr.ENC -= 20;
          if(Chr.ENC < 1) {
            Chr.ENC = 0;
          }
          var emm = await Helpers.Attack.MagicRollAsync(Mod, ctx.User.Id, Chr, Chr2);
          await ctx.RespondAsync(embed: emm);
        }
        [Command("Effect")]
        [Description("Add effects to other's characters")]
        public async Task EffectAsync(CommandContext ctx,[Description("The effect name")]string effect,
         [Description("Poison Amount")]int Mods, [Description("Target to add effects too.")]DiscordUser target)
        {
          var Target = target;
          var Accg = new AccountGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync($"{Target.Mention} has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          var c = Helpers.Effects.Paralysed;
          int turns = 2;
          switch (effect.ToLower())
          {
            case "poison":
            c = Helpers.Effects.Poison;
            turns = 3;
            break;
            case "paralyse":
            c = Helpers.Effects.Paralysed;
            turns = 2;
            break;
            case "weaken":
            c = Helpers.Effects.Weakened;
            turns = 4;
            break;
            default:
            await ctx.RespondAsync("Thats not one of our effects.");
            return;
          }
          var e = await Helpers.Attack.EffectsAsync(Chr.Entry,c,turns, Mods);
          if(!e)
            await ctx.RespondAsync("That person is already under an effect.");
          if(e)
            await ctx.RespondAsync($"You have given the effect {effect} to {target.Mention}");
          
        }
        [Command("Effect")]
        public async Task EffectAsync(CommandContext ctx, [Description("The effect name")]string effect, [Description("Target to add effects too.")]DiscordUser target)
        {
          var Target = target;
          var Accg = new AccountGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync($"{Target.Mention} has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          var c = Helpers.Effects.Paralysed;
          int turns = 2;
          switch (effect.ToLower())
          {
            case "poison":
            c = Helpers.Effects.Poison;
            turns = 3;
            break;
            case "paralyse":
            c = Helpers.Effects.Paralysed;
            turns = 2;
            break;
            case "weaken":
            c = Helpers.Effects.Weakened;
            turns = 4;
            break;
            default:
            await ctx.RespondAsync("Thats not one of our effects.");
            return;
          }
          var e = await Helpers.Attack.EffectsAsync(Chr.Entry,c,turns, 1);
          if(!e)
            await ctx.RespondAsync("That person is already under an effect.");
          if(e)
            await ctx.RespondAsync($"You have given the effect {effect} to {target.Mention}");
          
        }
        [Command("Effect")]
        public async Task EffectAsync(CommandContext ctx, [Description("The effect name")]string effect, 
        [Description("Target to add effects too."), RemainingText]string target)
        {
          var Chr = await ManageCharacter.GetChrByName(target);
          if(Chr == null)
          {
            await ctx.RespondAsync("No character by that name.");
            return;
          }
          var c = Helpers.Effects.Paralysed;
          int turns = 2;
          switch (effect.ToLower())
          {
            case "poison":
            c = Helpers.Effects.Poison;
            turns = 3;
            break;
            case "paralyse":
            c = Helpers.Effects.Paralysed;
            turns = 2;
            break;
            case "weaken":
            c = Helpers.Effects.Weakened;
            turns = 4;
            break;
            default:
            await ctx.RespondAsync("Thats not one of our effects.");
            return;
          }
          var e = await Helpers.Attack.EffectsAsync(Chr.Entry,c,turns, 1);
          if(!e)
            await ctx.RespondAsync("That person is already under an effect.");
          if(e)
            await ctx.RespondAsync($"You have given the effect {effect} to {Chr.Name}");
          
        }
        [Command("Effect")]
        public async Task EffectAsync(CommandContext ctx, [Description("The effect name")]string effect, 
        [Description("Poison Amount")]int Mods, [Description("Target to add effects too."), RemainingText]string target)
        {
          var Chr = await ManageCharacter.GetChrByName(target);
          var c = Helpers.Effects.Paralysed;
          int turns = 2;
          switch (effect.ToLower())
          {
            case "poison":
            c = Helpers.Effects.Poison;
            turns = 3;
            break;
            case "paralyse":
            c = Helpers.Effects.Paralysed;
            turns = 2;
            break;
            case "weaken":
            c = Helpers.Effects.Weakened;
            turns = 4;
            break;
            default:
            await ctx.RespondAsync("Thats not one of our effects.");
            return;
          }
          var e = await Helpers.Attack.EffectsAsync(Chr.Entry,c,turns, Mods);
          if(!e)
            await ctx.RespondAsync("That person is already under an effect.");
          if(e)
            await ctx.RespondAsync($"You have given the effect {effect} to {Chr.Name}");
          
        }
        [Command("Effect")]
        public async Task EffectAsync(CommandContext ctx, [Description("Target to view effects from")]DiscordUser target = null)
        {
          var Target = target ?? ctx.User;
          var Accg = new AccountGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync($"{Target.Mention} has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = Target.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
            var Eff = await Helpers.Attack.GetEffectsAsync(Chr.Entry);
            if(Eff == null)
            {
              await ctx.RespondAsync($"{Chr.Name} has no effects.");
              return;
            }
             await ctx.RespondAsync($"{Chr.Name} has the effect {Eff.Effect} active.");
          
        }
        [Command("Effect")]
        public async Task EffectAsync(CommandContext ctx, [Description("Target to view effects from"), RemainingText]string target)
        {
          var Chr = await ManageCharacter.GetChrByName(target);
            var Eff = await Helpers.Attack.GetEffectsAsync(Chr.Entry);
            if(Eff == null)
            {
              await ctx.RespondAsync($"{Chr.Name} has no effects.");
              return;
            }
             await ctx.RespondAsync($"{Chr.Name} has the effect {Eff.Effect} active.");
          
        }
        [Command("Heal")]
        public async Task HealAsync(CommandContext ctx, DiscordUser target = null)
        {
          target = target ?? ctx.User;
          var Accg = new AccountGet() {
            UID = target.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("Target has no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = target.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
          if(Chr.HPC > 0 && await Helpers.Owners.CheckAsync(ctx.User.Id))
          {
            Chr.HPC = Chr.HPM;
            Chr.ENC = Chr.ENM;
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync($"**{Chr.Name} has been healed**");
            return;
          }
          else if(Chr.HPC < 1)
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
        public async Task HealAsync(CommandContext ctx, [RemainingText] string target)
        {
          var Chr = await ManageCharacter.GetChrByName(target);
          if(Chr == null)
          {
            await ctx.RespondAsync("Does not exist.");
            return;
          }
          if(Chr.HPC > 0 && await Helpers.Owners.CheckAsync(ctx.User.Id))
          {
            Chr.HPC = Chr.HPM;
            Chr.ENC = Chr.ENM;
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync($"**{Chr.Name} has been healed**");
            return;
          }
          else if(Chr.HPC < 1 && await Helpers.Owners.CheckAsync(ctx.User.Id))
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
        [Cooldown(1,4321, CooldownBucketType.User)]
        public async Task HealAsync(CommandContext ctx)
        {
          var target = ctx.User;
          var Accg = new AccountGet() {
            UID = target.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          if(Check == null)
          {
            await ctx.RespondAsync("You have no characters.");
            return;
          }
          var OwnGet = new OwnerGet() {
            UID = target.Id,
            GID = ctx.Guild.Id,
            Slot = Check.Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          var ChrGet = new ChrGet() {
            Entry = Own.CharEntry,
          };
          var Chr = await ManageCharacter.GetAll(ChrGet);
            Chr.HPC = Chr.HPM;
            Chr.ENC = Chr.ENM;
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync($"**{Chr.Name} has been healed**");
            return;
        }
	}
}
