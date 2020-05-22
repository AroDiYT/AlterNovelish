using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

using Newtonsoft.Json;

using BotTemplate.Managers;
using BotTemplate.Objects.Json;

using BotTemplate.Objects.Sql.Profile;
using System.Collections.Concurrent;

namespace BotTemplate.Helpers 
{
    public class Owners {
        public static ConcurrentDictionary<ulong, string> Owner { get; private set; }
        public static void InitCache()
        {
            Owner = new ConcurrentDictionary<ulong, string>();
            Owner.TryAdd(694104913210245240, "Owner");
            Owner.TryAdd(339475044172431360,"CoOwner");
            Owner.TryAdd(434824856161484800, "Staff");
            }
        public static async Task<bool> CheckAsync(ulong ID)
        {
            if(Owner.TryGetValue(ID, out var res))
            {
                return true;
            }
            return false;
        }
    }
    public class Tools {
        public static Random RNG = new Random();
    }
    public class Roll {
        public int Attack { get; set; }
        public int Defend { get; set; }
        public int RNGA { get; set; }
        public int RNGB { get; set; }
    }
    public enum Effects {
        Poison = 0,
        Weakened = 1,
        Paralysed = 2
    }
    public enum CastEffects {
        burn,
        bleed,
        stun,
        weaken,
        drain
    }
    public class InEffect {
        public int Author { get; set; }
        public Effects Effect { get; set; }
        public int Turns { get; set; }
        public int Modifiers { get; set; }
    }
    
    public static class Attack {
        public static ConcurrentDictionary<int, InEffect> InEffect { get; private set; }
        public static void InitCache()
        {
            InEffect = new ConcurrentDictionary<int, InEffect>();
        }
        public static async Task<DiscordEmbed> RollAsync(Modifiers Mod, ulong Author, ulong Target, Chr Chr, Chr TrChr)
        {
            var Base = 5;
            
            var A = Tools.RNG.Next(12) + 1;
            var B = Tools.RNG.Next(12) + 1;

            var OA = A;
            var OB = B;

            A += Mod.Attack;
            B += Mod.Defend;
            var Effected = "Effects:";
            var Effect = await Helpers.Attack.DoEffects(Chr, Effected); 
            if(Effect != null) {
                Chr = Effect.Chr;
            if(Effect.Effect == Effects.Paralysed)
             A = 1;
            if(Effect.Effect == Effects.Weakened)
            {
                A = A/2;
                OA = OA/2;
            }
            }
            else
            Effect.Effected = "No effects";

            var Embed = new DiscordEmbedBuilder();
            string Rolls = $"{Chr.Name} rolled {OA} + {A - OA}\nAgainst\n{TrChr.Name} rolled {OB} + {Mod.Defend}";
            Embed = Embed.WithAuthor(Rolls);
            Embed = Embed.AddField("​",Effect.Effected);
            if(OA == 1)
            {
                Embed = Embed.WithDescription("Critically Failed, Nothing happened.");
            }
            else if(A > B)
            {
                if(OA == 12)
                    Chr.Strenght += 5;
                TrChr.HPC -= Chr.Strenght + (Mod.DamageBonus) + Base;
                if(TrChr.HPC < 0)
                        TrChr.HPC = 0;
                Embed = Embed.WithDescription($"Succes;\n\n`{TrChr.Name}` **lost** `{Chr.Strenght + Base}` **hp, now at** `{TrChr.HPC}|{TrChr.HPM}`");
                await ManageCharacter.UpdateAsync(TrChr);
                if(OA == 12)
                    Chr.Strenght -= 5;
                Chr.XP += 4;
                if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
					{
                        var olvl = Chr.Level;
						begin:
						Chr.XP -= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level);
						Chr.Level += 1;
						Chr.SP += 5;
						if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
						goto begin;
						await ManageCharacter.UpdateAsync(Chr);
                        Embed = Embed.WithFooter("You've leveled up!");
					}
                else
                await ManageCharacter.UpdateAsync(Chr);
            }
            else if (A == B)
            {
                Embed = Embed.WithDescription("Both sides are even in their strenght and no damage has been done.");
            }
            else
            {
                Embed = Embed.WithDescription("Failed, Nothing happened.");
            }
            return Embed;
        }
        public static async Task<DiscordEmbed> MagicRollAsync(Modifiers Mod, ulong Author, ulong Target, Chr Chr, Chr TrChr)
        {
            var Base = 5;
            
            var A = Tools.RNG.Next(12) + 1;
            var B = Tools.RNG.Next(12) + 1;

            var OA = A;
            var OB = B;

            A += Mod.Attack;
            B += Mod.Defend;

            var Effected = "Effects:";
            var Effect = await Helpers.Attack.DoEffects(Chr, Effected); 
            if(Effect != null) {
                Chr = Effect.Chr;
            if(Effect.Effect == Effects.Paralysed)
             A = 1;
            if(Effect.Effect == Effects.Weakened)
            {
                A = A/2;
                OA = OA/2;
            }
            }
            else
            Effect.Effected = "No effects";

            var Embed = new DiscordEmbedBuilder();
            string Rolls = $"{Chr.Name} rolled {OA} + {A - OA}\nAgainst\n{TrChr.Name} rolled {OB} + {Mod.Defend}";
            Embed = Embed.WithAuthor(Rolls);
            Embed = Embed.AddField("​",Effect.Effected);
            if(OA == 1)
            {
                Embed = Embed.WithDescription("Critically Failed, Nothing happened.");
            }
            else if(A > B)
            {
                if(OA == 12)
                    Chr.MagicEff += 5;
                TrChr.HPC -= Chr.Strenght + (Mod.DamageBonus) + Base;
                if(TrChr.HPC < 0)
                        TrChr.HPC = 0;
                Embed = Embed.WithDescription($"Succes;\n\n`{TrChr.Name}` **lost** `{Chr.MagicEff + Base}` **hp, now at** `{TrChr.HPC}|{TrChr.HPM}`");
                await ManageCharacter.UpdateAsync(TrChr);
                if(OA == 12)
                    Chr.MagicEff -= 5;
                Chr.XP += 4;
                if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
					{
                        var olvl = Chr.Level;
						begin:
						Chr.XP -= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level);
						Chr.Level += 1;
						Chr.SP += 5;
						if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
						goto begin;
						await ManageCharacter.UpdateAsync(Chr);
                        Embed = Embed.WithFooter("You've leveled up!");
					}
                else
                await ManageCharacter.UpdateAsync(Chr);
            }
            else if (A == B)
            {
                Embed = Embed.WithDescription("Both sides are even in their strenght and no damage has been done.");
            }
            else
            {
                Embed = Embed.WithDescription("Failed, Nothing happened.");
            }
            return Embed;
        }
        public static async Task<DiscordEmbed> RollAsync(Modifiers Mod, ulong Author, Chr Chr, Chr TrChr)
        {
            var Base = 5;
            
            var A = Tools.RNG.Next(12) + 1;
            var B = Tools.RNG.Next(12) + 1;

            var OA = A;
            var OB = B;

            A += Mod.Attack;
            B += Mod.Defend;

            var Effected = "Effects:";
            var Effect = await Helpers.Attack.DoEffects(Chr, Effected); 
            if(Effect != null) {
                Chr = Effect.Chr;
            if(Effect.Effect == Effects.Paralysed)
             A = 1;
            if(Effect.Effect == Effects.Weakened)
            {
                A = A/2;
                OA = OA/2;
            }
            }
            else
            Effect.Effected = "No effects";

            var Embed = new DiscordEmbedBuilder();
            string Rolls = $"{Chr.Name} rolled {OA} + {A - OA}\nAgainst\n{TrChr.Name} rolled {OB} + {Mod.Defend}";
            Embed = Embed.WithAuthor(Rolls);
            Embed = Embed.AddField("​",Effect.Effected);
            if(OA == 1)
            {
                Embed = Embed.WithDescription("Critically Failed, Nothing happened.");
            }
            else if(A > B)
            {
                if(OA == 12)
                    Chr.Strenght += 5;
                TrChr.HPC -= Chr.Strenght + (Mod.DamageBonus) + Base;
                if(TrChr.HPC < 0)
                        TrChr.HPC = 0;
                Embed = Embed.WithDescription($"Succes;\n\n`{TrChr.Name}` **lost** `{Chr.Strenght + Base}` **hp, now at** `{TrChr.HPC}|{TrChr.HPM}`");
                await ManageCharacter.UpdateAsync(TrChr);
                if(OA == 12)
                    Chr.Strenght -= 5;
                Chr.XP += 4;
                if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
					{
                        var olvl = Chr.Level;
						begin:
						Chr.XP -= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level);
						Chr.Level += 1;
						Chr.SP += 5;
						if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
						goto begin;
						await ManageCharacter.UpdateAsync(Chr);
                        Embed = Embed.WithFooter("You've leveled up!");
					}
                else
                await ManageCharacter.UpdateAsync(Chr);
            }
            else if (A == B)
            {
                Embed = Embed.WithDescription("Both sides are even in their strenght and no damage has been done.");
            }
            else
            {
                Embed = Embed.WithDescription("Failed, Nothing happened.");
            }
            return Embed;
        }
         public static async Task<DiscordEmbed> MagicRollAsync(Modifiers Mod, ulong Author, Chr Chr, Chr TrChr)
        {
            var Base = 5;
            
            var A = Tools.RNG.Next(12) + 1;
            var B = Tools.RNG.Next(12) + 1;

            var OA = A;
            var OB = B;

            A += Mod.Attack;
            B += Mod.Defend;

            var Effected = "Effects:";
            var Effect = await Helpers.Attack.DoEffects(Chr, Effected); 
            if(Effect != null) {
                Chr = Effect.Chr;
            if(Effect.Effect == Effects.Paralysed)
             A = 1;
            if(Effect.Effect == Effects.Weakened)
            {
                A = A/2;
                OA = OA/2;
            }
            }
            else
            Effect.Effected = "No effects";

            var Embed = new DiscordEmbedBuilder();
            string Rolls = $"{Chr.Name} rolled {OA} + {A - OA}\nAgainst\n{TrChr.Name} rolled {OB} + {Mod.Defend}";
            Embed = Embed.WithAuthor(Rolls);
            Embed = Embed.AddField("​",Effect.Effected);
            if(OA == 1)
            {
                Embed = Embed.WithDescription("Critically Failed, Nothing happened.");
            }
            else if(A > B)
            {
                if(OA == 12)
                    Chr.MagicEff += 5;
                TrChr.HPC -= Chr.Strenght + (Mod.DamageBonus) + Base;
                if(TrChr.HPC < 0)
                        TrChr.HPC = 0;
                Embed = Embed.WithDescription($"Succes;\n\n`{TrChr.Name}` **lost** `{Chr.MagicEff + Base}` **hp, now at** `{TrChr.HPC}|{TrChr.HPM}`");
                await ManageCharacter.UpdateAsync(TrChr);
                if(OA == 12)
                    Chr.MagicEff -= 5;
                Chr.XP += 4;
                if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
					{
                        var olvl = Chr.Level;
						begin:
						Chr.XP -= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level);
						Chr.Level += 1;
						Chr.SP += 5;
						if(Chr.XP >= Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level))
						goto begin;
						await ManageCharacter.UpdateAsync(Chr);
                        Embed = Embed.WithFooter("You've leveled up!");
					}
                else
                await ManageCharacter.UpdateAsync(Chr);
            }
            else if (A == B)
            {
                Embed = Embed.WithDescription("Both sides are even in their strenght and no damage has been done.");
            }
            else
            {
                Embed = Embed.WithDescription("Failed, Nothing happened.");
            }
            return Embed;
        }

        public static async Task<bool> EffectsAsync(int Author, Effects Effect, int Turns, int Mods)
        {
            if(InEffect.ContainsKey(Author))
            {
                return false;
            }
            var EffC = new InEffect() {
                Author = Author,
                Effect = Effect,
                Turns = Turns,
                Modifiers = Mods
            };
            InEffect.TryAdd(Author, EffC);
            return true;
        }
        public class DoEff {
            public string Effected { get; set; }
            public Effects Effect { get; set; }
            public Chr Chr { get; set; }
        }
        public static async Task<DoEff> DoEffects(Chr Chr, string Effected)
        {
           var Doeff = new DoEff();
           Doeff.Chr = Chr;
            if(InEffect.TryGetValue(Chr.Entry, out var Eff))
            {
                Eff.Turns -= 1;
                if(Eff.Turns > 0)
                InEffect.TryUpdate(Chr.Entry, Eff, null);
                if(Eff.Turns <= 0)
                InEffect.TryRemove(Chr.Entry, out _);
                switch(Eff.Effect)
                {
                    case Effects.Paralysed:
                    Effected += $"\n{Chr.Name} is paralysed and can't attack.";
                    Doeff.Effect = Effects.Paralysed;
                    Doeff.Effected = Effected;
                    return Doeff;
                    case Effects.Poison:
                    Effected += $"\n{Chr.Name} is poisoned and loses {Eff.Modifiers} HP.";
                    Chr.HPC -= Eff.Modifiers;
                    if(Chr.HPC < 0)
                        Chr.HPC = 0;
                    await ManageCharacter.UpdateAsync(Chr);
                    Doeff.Chr = Chr;
                    Doeff.Effect = Effects.Poison;
                    Doeff.Effected = Effected;
                    return Doeff;
                    case Effects.Weakened:
                    Effected += $"\n{Chr.Name} is Weakened and loses half his strenght. (roll and damage lowered.)";
                    Doeff.Effect = Effects.Weakened;
                    Doeff.Effected = Effected;
                    return Doeff;
                }
                return null;
            }
            return null;
        }
        public static async Task<InEffect> GetEffectsAsync(int Author)
        {
            if(!InEffect.ContainsKey(Author))
                return null;
            var res = new InEffect();
            InEffect.TryGetValue(Author, out res);
            return res;
        }
    }
    public static class Cast {
        public static async Task Heal(int Value, Chr Chr)
        {
            Chr.HPC += Value;
            if(Chr.HPC > Chr.HPM)
            Chr.HPC = Chr.HPM;
            await ManageCharacter.UpdateAsync(Chr);
        }
        public static async Task Damage(int Value, Chr Chr)
        {
            Chr.HPC -= Value;
            if(Chr.HPC < 0)
            Chr.HPC = Chr.HPM;
            await ManageCharacter.UpdateAsync(Chr);
        }
        public static async Task Effect(CastEffects Effect)
        {
            switch (Effect)
            {
                case CastEffects.bleed:

                break;
                case CastEffects.burn:

                break;
                case CastEffects.drain:

                break;
                case CastEffects.stun:

                break;
                case CastEffects.weaken:
                
                break;
            }
        }
    }
}