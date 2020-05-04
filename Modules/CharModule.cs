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
	[Group("character"), Aliases("char")]
	[Description("Show profile data.")]
	public class CharModule : BaseCommandModule {
		[GroupCommand()]
		public async Task ViewAsync(CommandContext ctx, [RemainingText] string name) {
			var cc = new Character(); 
			var sc = new Stats();
			if (string.IsNullOrEmpty(name))
			{
				cc = await CharManager.GetAsync(ctx.User.Id);
				if (cc == null)
				throw new UserException($"No character");
				sc = await StatsManager.GetAsync(ctx.User.Id);
			}
			else
			{
				cc = await CharManager.SearchAsync(name);
				if (cc == null)
				throw new UserException($"No character: {name}");
				sc = await StatsManager.GetAsync(cc.PID);
			}
  				 
			
			var embeds = new DiscordEmbedBuilder
                {
                    Description = $"**Name**: `{cc.Name}`\n**Age**: `{cc.Age}`\n**Gender**: `{cc.Gender}`\n**Description**: `{cc.Desc}`\n\n**Level**: `{cc.Level}` **[**`{cc.Xp}`**/**`{cc.Level*100/2*4}`**]**\n\n\n`From:` <@{cc.PID}>",
					ThumbnailUrl = cc.Ref,
                    Color = DiscordColor.Blue
                };
			embeds = embeds.AddField("Stats", $"`{sc.HP_current}`**/**`{sc.HP_max}` **HP**\n\n**Attack Damage:** `{sc.ATK}`\n**Melee(**__Hit__**):** `{sc.MELEE}`\n**Ranged(**__Hit__**):** `{sc.RANGED}`\n\n**Magic:** `{sc.MAGIC}`\n `{sc.Energy_current}`**/**`{sc.Energy_max}` **ENERGY**");
          await ctx.RespondAsync(embed : embeds);
		}
		[Command("setup")]
		[Description("Sets your profile data by asking questions.")]
		[Cooldown(1,100,CooldownBucketType.User)]
		public async Task SetupAsync(CommandContext ctx) {
			var chr = new Character() {
				PID = ctx.User.Id,
			};
			var overwrites = new DiscordOverwriteBuilder[] {
				new DiscordOverwriteBuilder().Allow(Permissions.AccessChannels).For(ctx.Member),
				new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(ctx.Guild.EveryoneRole)
			};

			var ch = await ctx.Guild.CreateTextChannelAsync($"{ctx.User.Username}{ctx.User.Discriminator}",
					overwrites: overwrites);
			await ch.SendMessageAsync($"{ctx.Member.Mention} entered setup mode. You will be asked questions which you then respond to.\n"
									  + "Messages starting with `;` are ignored.");
			async Task suicide(string why = "") {
				await ch.SendMessageAsync($"{why ?? ""} Stopping setup.");
				await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
				await Task.Delay(10 * 1000);
				await ch.DeleteAsync();
			};
			var r = await Interactivity.WaitForAnswerAsync(ctx, "What is your character's name?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
			chr.Name = r.Content;
			int res = await Interactivity.WaitForAnswerINTAsync(ctx, "What is your character's age?", channel: ch);
			if (res == 0) {
				await suicide();
				return;
			}
            chr.Age = res.ToString();
            r = await Interactivity.WaitForAnswerAsync(ctx, "What is your character's gender?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
			if(r.Content.ToLower().StartsWith("m"))
            {
                chr.Gender = "Male";
            }
            else if(r.Content.ToLower().StartsWith("f"))
            {
                chr.Gender = "Female";
            }
            else
            {
                chr.Gender = "Fish";
            }
			r = await Interactivity.WaitForAnswerAsync(ctx, "What is your character's Description?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
			chr.Desc = r.Content;
			r = await Interactivity.WaitForAnswerAsync(ctx, $"Now what is your appearance? (upload an image, a url to an image, or 'none')",
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
                chr.Ref = ms.Attachments.FirstOrDefault().Url;
}
			} else if (r.Content.ToLowerInvariant() != "none") {
				if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
					&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
					chr.Ref = r.Content;
				} else {
					await suicide("Invalid Uri.");
					return;
				}
			} else {
				chr.Ref = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
			}
			var sc = await StatsManager.GetAsync(ctx.User.Id);
			if(sc == null) {
				var ccs = new Currency() {
					PID = ctx.User.Id,
					Balance = 100
				};
				await CurrencyManager.SyncAsync(ccs);
			
			int sps = 6;
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
				SP = 0
			};
			while(sps > 0) {
				r = await Interactivity.WaitForAnswerAsync(ctx, $"You have {sps} Statpoints, Where will you add one?\nATK (Attack damage), " 
				+ "Melee (Hitrate Melee weapons), RANGED (Hitrate Ranged Weapons), MAGIC (Magic Damage), HP (Max Health), Energy (Max Energy for Magic)",
				 channel: ch);
				if (r == null) {
					await suicide();
					return;
				}
				var st = new string[] {"HP","ENERGY","MAGIC","RANGED","MELEE","ATK"};
				if(st.ToArray().Contains(r.Content.ToUpper()))
				{
					if(r.Content.ToUpper() == "HP")
						stt.HP_max += 1;
					if(r.Content.ToUpper() == "ATK")
						stt.ATK += 1;
					if(r.Content.ToUpper() == "MELEE")
						stt.MELEE += 1;
					if(r.Content.ToUpper() == "RANGED")
						stt.RANGED += 1;
					if(r.Content.ToUpper() == "MAGIC")
						stt.MAGIC += 1;
					if(r.Content.ToUpper() == "ENERGY")
						stt.Energy_max += 1;
					sps -= 1;
				}
				else
				{
					r = await Interactivity.WaitForAnswerAsync(ctx, $"You have {sps} Statpoints, Where will you add one?\nATK (Attack damage), " 
					+ "Melee (Hitrate Melee weapons), RANGED (Hitrate Ranged Weapons), MAGIC (Magic Damage), HP (Max Health), Energy (Max Energy for Magic)",
					channel: ch);
				}
			}
			await StatsManager.SyncAsync(stt);
			}
			await CharManager.SyncAsync(chr);
			await ctx.RespondAsync($"Done. view with `&char {chr.Name}`");
			await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
			await Task.Delay(10 * 1000);
			await ch.DeleteAsync();
		}
		[Command("Heal")]
		public async Task HealAsync(CommandContext ctx, DiscordUser user = null)
		{
			DiscordUser auth = user ?? ctx.User;
			var cc = await StatsManager.GetAsync(auth.Id);
			if(cc == null)
			 return;
			if(cc.HP_current > 1) {
				if(ctx.User.Id.Equals(694104913210245240) || ctx.User.Id.Equals(339475044172431360))
				{
					await StatsManager.HealAsync(auth.Id);
					await ctx.RespondAsync($"{auth.Mention} has been healed");
				}
				else
				{
					await ctx.RespondAsync("You do not have permission to use this. Only if you are dead you can heal yourself.");
				}
			}
			else if(auth.Id.Equals(ctx.User.Id))
			{
				await StatsManager.HealAsync(auth.Id);
				await ctx.RespondAsync($"{auth.Mention} has been healed");
			}
			else
			{
				await ctx.RespondAsync("You do not have permission to use this. Only if you are dead you can heal yourself.");
			}
			
		}
		[Command("Rest")]
		[Cooldown(1,2*(60*60),CooldownBucketType.User)]
		public async Task RestAsync(CommandContext ctx)
		{
			DiscordUser auth = ctx.User;
			var cc = await StatsManager.GetAsync(auth.Id);
			if(cc == null)
			 return;
			await StatsManager.RestAsync(auth.Id);
			await ctx.RespondAsync("You have rested a little, restoring some Health and Energy");
		}
		[Command("Enhance"), Aliases("up")]
		public async Task UpAsync(CommandContext ctx)
		{
			DiscordChannel ch = ctx.Channel;
			var cc = await StatsManager.GetAsync(ctx.User.Id);
			if(cc == null)
			{
				await ctx.RespondAsync("You don't have a character");
				return;
			}
			var stt = cc;
			var sps = cc.SP;
			if(sps < 1)
			{
				await ctx.RespondAsync("You don't have any statpoints.");
				return;
			}
			while(sps > 0) {
				var r = await Interactivity.WaitForAnswerAsync(ctx, $"You have {sps} Statpoints, Where will you add one?\nATK (Attack damage), " 
				+ "Melee (Hitrate Melee weapons), RANGED (Hitrate Ranged Weapons), MAGIC (Magic Damage), HP (Max Health), Energy (Max Energy for Magic)",
				 channel: ch);
				if (r == null) {
					await ctx.RespondAsync("No response.");
					return;
				}
				var st = new string[] {"HP","ENERGY","MAGIC","RANGED","MELEE","ATK"};
				if(st.ToArray().Contains(r.Content.ToUpper()))
				{
					if(r.Content.ToUpper() == "HP")
						stt.HP_max += 1;
					if(r.Content.ToUpper() == "ATK")
						stt.ATK += 1;
					if(r.Content.ToUpper() == "MELEE")
						stt.MELEE += 1;
					if(r.Content.ToUpper() == "RANGED")
						stt.RANGED += 1;
					if(r.Content.ToUpper() == "MAGIC")
						stt.MAGIC += 1;
					if(r.Content.ToUpper() == "ENERGY")
						stt.Energy_max += 1;
					sps -= 1;
					cc.SP -= 1;
				}
				else
				{
					r = await Interactivity.WaitForAnswerAsync(ctx, $"You have {sps} Statpoints, Where will you add one?\nATK (Attack damage), " 
					+ "Melee (Hitrate Melee weapons), RANGED (Hitrate Ranged Weapons), MAGIC (Magic Damage), HP (Max Health), Energy (Max Energy for Magic)",
					channel: ch);
				}
			}
			await StatsManager.SyncAsync(stt);
			await ctx.RespondAsync("Done");
		}
		[Command("Cache")]
		[Description("This reloads your profiles cache.")]
		public async Task CacheAsync(CommandContext ctx, DiscordUser user = null)
		{
			DiscordUser v = user ?? ctx.User;
			await StatsManager.CacheAsync(v.Id);
			await CharManager.CacheAsync(v.Id);
			await ctx.RespondAsync($"{v.Mention}'s profile has been reloaded.");
		}
		
    }
}