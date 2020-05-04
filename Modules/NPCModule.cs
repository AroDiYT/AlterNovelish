using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql;

namespace BotTemplate.Modules {
    [Group("npc"), Aliases("proxy")]
	[Description("Become the NPC")]
	public class NPCModule : BaseCommandModule {
        [GroupCommand]
        public async Task NPCViewAsync(CommandContext ctx, [RemainingText] string name)
        {
          var note = await NPCManager.SearchAsync(name);
			if (note == null)
				throw new UserException($"No such NPC: {name}");

			var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**Name**: `{note.Name}`\n**Description**: `{note.Desc}`",
					ThumbnailUrl = note.Ref,
                    Color = DiscordColor.Blue
                };
            embeds = embeds.AddField("Info", $"**`Damage:`** -> __{note.ATK}__\n\n**`Roll Bonus`** **(**__{note.MBonus}__**)Melee**\n**(**__{note.RBonus}__**)Ranged**\n**(**__{note.MagBonus}__**)Magic**" 
            +$"\n\n**`Health`** __{note.HP_current}__**/**__{note.HP_max}__");
            await ctx.RespondAsync(embed : embeds);
        }
        [Command("to")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Description("Speak to a specific channel")]
        public async Task NPCSelfAsync(CommandContext ctx, [RemainingText] string name)
        {
         if(name == "self")
         {
             NPCManager.ProxySelfAsync(ctx.User.Id);
         }
          var note = await NPCManager.SearchAsync(name);
			if (note == null)
				throw new UserException($"No such NPC: {name}");

            await NPCManager.ProxyAsync(name, ctx.User.Id);
            await ctx.RespondAsync("You are now proxied as " + note.Name);
        }
        [Command("Heal")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task NpcHealAsync(CommandContext ctx, [RemainingText] string name)
        {
            var note = await NPCManager.SearchAsync(name);
			if (note == null)
				throw new UserException($"No such NPC: {name}");
            await NPCManager.HealAsync(note.NPCID);
            await ctx.RespondAsync("NPC HAS BEEN HEALED, NAME: " + note.Name);
        }
        [Command("new")]
        [Description("Create an npc")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task NPCNewAsync(CommandContext ctx)
        {
            var npc = new NPC();
            var overwrites = new DiscordOverwriteBuilder[] {
				new DiscordOverwriteBuilder().Allow(Permissions.AccessChannels).For(ctx.Member),
				new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(ctx.Guild.EveryoneRole)
			};

			var ch = await ctx.Guild.CreateTextChannelAsync($"{ctx.User.Username}{ctx.User.Discriminator}-npc",
					overwrites: overwrites);
            await ch.SendMessageAsync($"{ctx.User.Mention}, here you will be creating an NPC");
            async Task suicide(string why = "") {
				await ch.SendMessageAsync($"{why ?? ""} Stopping setup.");
				await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
				await Task.Delay(10 * 1000);
				await ch.DeleteAsync();
			};
            var r = await Interactivity.WaitForAnswerAsync(ctx, "Tell me the NPC's name?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.Name = r.Content;
            r = await Interactivity.WaitForAnswerAsync(ctx, "Tell me the NPC's description?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.Desc = r.Content;
            r = await Interactivity.WaitForAnswerAsync(ctx, $"Now what is your NPC's appearance? (upload an image, a url to an image, or 'none')",
													   1000 * 60 * 60, ch);
			if (r == null) {
				await suicide();
				return;
			}
			if (string.IsNullOrEmpty(r.Content)) {
				if (r.Attachments.Count == 0) {
					await suicide("No image given.");
					return;
				}
                using (WebClient webClient = new WebClient()) 
{
                webClient.DownloadFile(r.Attachments.FirstOrDefault().Url, "./image.png");
                DiscordChannel cc = await ctx.Client.GetChannelAsync(705752802806202427);
                DiscordMessage ms = await cc.SendFileAsync("./image.png", ctx.User.Username, false, null, null);
                npc.Ref = ms.Attachments.FirstOrDefault().Url;
}
			} else if (r.Content.ToLowerInvariant() != "none") {
				if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
					&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
					npc.Ref = r.Content;
				} else {
					await suicide("Invalid Uri.");
					return;
				}
			} else {
				npc.Ref = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
			}
            var rs = await Interactivity.WaitForAnswerINTAsync(ctx, "Tell me the NPC's Melee Bonus", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.MBonus = rs;
            rs = await Interactivity.WaitForAnswerINTAsync(ctx, "Tell me the NPC's Ranged Bonus", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.RBonus = rs;
            rs = await Interactivity.WaitForAnswerINTAsync(ctx, "Tell me the NPC's Magic Bonus", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.MagBonus = rs;
            rs = await Interactivity.WaitForAnswerINTAsync(ctx, "Tell me the NPC's Damage", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.ATK = rs;
            rs = await Interactivity.WaitForAnswerINTAsync(ctx, "Tell me the NPC's Health", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
            npc.HP_current = rs;
            npc.HP_max = rs;
            var slot = await NPCManager.CountAsync();
            npc.NPCID = slot;
            await NPCManager.SyncAsync(npc);
            await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
			await Task.Delay(10 * 1000);
			await ch.DeleteAsync();
		
        }
    }
}
		