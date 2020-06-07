using System;
using System.Threading.Tasks;
using System.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules {
	public class Emotes : BaseCommandModule {
        [Command("kiss")]
        [Description("Kiss Someone")]
        public async Task Kiss(CommandContext ctx, DiscordUser Target)
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
            Accg = new AccountGet() {
              UID = Target.Id,
              GID = ctx.Guild.Id
            };
            Check = await ManageCharacter.GetAll(Acc: Accg);
            if(Check == null)
            {
              await ctx.RespondAsync("Target have no characters.");
              return;
            }
            OwnGet = new OwnerGet() {
              UID = Target.Id,
              GID = ctx.Guild.Id,
              Slot = Check.Slot
            };
            Own = await ManageCharacter.GetAll(OwnGet);
            ChrGet = new ChrGet() {
              Entry = Own.CharEntry,
            };
            var Chr2 = await ManageCharacter.GetAll(ChrGet);
          await ctx.RespondAsync($"**{Chr.Name}** *kisses* **{Chr2.Name}**");
        }
        [Command("kiss")]
        public async Task Kiss(CommandContext ctx, [RemainingText] string Target)
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
            var Chr2 = await ManageCharacter.GetChrByName(Target);
            if(Chr2 == null)
            {
              await ctx.RespondAsync("No character found by that name.");
              return;
            }
          await ctx.RespondAsync($"**{Chr.Name}** *kisses* **{Chr2.Name}**");
        }
        [Command("Suicide")]
        public async Task Suicide(CommandContext ctx)
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
          await ctx.RespondAsync($"**{Chr.Name}** *Kills themself*");
        }
    }
}
