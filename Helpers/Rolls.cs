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
    public class Owners
    {
        public static ConcurrentDictionary<ulong, string> Owner { get; private set; }
        public static void InitCache()
        {
            Owner = new ConcurrentDictionary<ulong, string>();
            Owner.TryAdd(694104913210245240, "Owner");
            Owner.TryAdd(339475044172431360, "CoOwner");
            Owner.TryAdd(434824856161484800, "Staff");
        }
        public static async Task<bool> CheckAsync(ulong ID)
        {
            if (Owner.TryGetValue(ID, out var res))
            {
                return true;
            }
            return false;
        }
    }
    public class Tools
    {
        public static Random RNG = new Random();
    }
    public enum FightOptions
    {
        Melee,
        Ranged,
        Magic
    }

    public static class Attack
    {
        public static async Task<DiscordEmbed> RollAsync(Chr Author, Chr Target, FightOptions Option)
        {
            //Modifiers + Predefining.
            int Attribute = 0;
            bool Magic = false;
            int Dodge = Target.Dodge;
            switch (Option)
            {
                case FightOptions.Melee:
                    Attribute += Author.Sleight;
                    break;
                case FightOptions.Ranged:
                    Attribute += Author.Marksman;
                    break;
                case FightOptions.Magic:
                    Attribute += Author.Magic;
                    Magic = true;
                    break;
            }
            //RNG Innit
            var AuthorRoll = Helpers.Tools.RNG.Next(12) + 1;
            var TargetRoll = Helpers.Tools.RNG.Next(12) + 1;
            //Roll Msg #1
            string Roll = $"{Author.Name} rolls ({Option}) against {Target.Name}";
            string RollInit = $"`({AuthorRoll} + {Attribute})` **vs** `({TargetRoll} + {Dodge})`";
            //Switch Win/Loss
            var AuthorFull = AuthorRoll + Attribute;
            var TargetFull = TargetRoll + Dodge;

            string Situation = "";
            int Visual = 0;

            int Damage = Author.Strenght + Helpers.Tools.RNG.Next(20) + 1;
            if (Magic == true)
                Damage = Author.MagicEff + Helpers.Tools.RNG.Next(15) + 1;

            if(Author.EffectPath != 0)
            {
                if(Author.EffectPath == EffectPath.boost)
                Damage += Author.EffectAtribute;
                if(Author.EffectPath == EffectPath.weaken)
                Damage -= Author.EffectAtribute;
                if(Damage < 0)
                Damage = 0;
                Author.EffectAtribute = 0;
                Author.EffectPath = 0;
                await ManageCharacter.UpdateAsync(Author);
            }
            if (AuthorFull == 0)
            {
                Situation = $"{Author.Name} didn't even get near {Target.Name}";
                Visual += 10;
                Damage = 0;
            }
            else if (AuthorFull > TargetFull)
            {
                if (AuthorRoll == 12)
                    Damage += 5;
                Situation = $"`{Author.Name}` **succesfully hitted** `{Target.Name}` **for** `{Damage}` **damage**";
                Visual += 2;
                if (Magic == true)
                {
                    if (Author.ENC < 10)
                    {
                        Situation = "You do not have enough stamina to use Magic Attacks.";
                        Visual += 10;
                        Damage = 0;
                    }
                }
            }
            else if (AuthorFull == TargetFull)
            {
                Situation = $"{Author.Name} went on even grounds as {Target.Name}";
                Visual += 10;
                Damage = 0;
            }
            else
            {
                Situation = $"{Author.Name} couldn't hit {Target.Name}";
                Visual += 10;
                Damage = 0;
            }
            //Create Msg
            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithAuthor(Roll);
            Embed = Embed.WithDescription(RollInit + "\n\n" + Situation + "\n" + $"**{Target.Name}** has `{Target.HPC - Damage + "|" + Target.HPM} HP` left");
            //CheckIfDamage
            if(Visual == 2)
            {

                    Target.HPC -= Damage;

                    if (Target.HPC < 1)
                    {
                        Target.HPC = 0;
                        Embed = Embed.WithFooter($"{Target.Name} died.");
                    }

                    await ManageCharacter.UpdateAsync(Target);

                    if (Magic == true)
                    {
                        Author.ENC -= 10;
                        await ManageCharacter.UpdateAsync(Author);
                    }
            }
            //End Return
            return Embed;
        }
    }
    public static class Cast
    {
        public static async Task Heal(int Value, Chr Chr)
        {
            Chr.HPC += Value;
            if (Chr.HPC > Chr.HPM)
                Chr.HPC = Chr.HPM;
            await ManageCharacter.UpdateAsync(Chr);
        }
        public static async Task Damage(int Value, Chr Chr)
        {
            Chr.HPC -= Value;
            if (Chr.HPC < 0)
                Chr.HPC = Chr.HPM;
            await ManageCharacter.UpdateAsync(Chr);
        }
    }
}
