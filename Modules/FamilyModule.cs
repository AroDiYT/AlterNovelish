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
	[Group("Marriage"), Aliases("marry")]
	[Description("Propose to your love-")]
	public class FamilyModule : BaseCommandModule {
		[GroupCommand]
		public async Task MainAsync(CommandContext ctx, [Description("Victi- i mean loved one.")]DiscordUser User)
		{
			DiscordUser Victim = User;
			var r = await Interactivity.WaitForAnswerAsync(ctx, $"<@{User.Id}>, Do you accept <@{ctx.User.Id}>'s request to marry?", 120000, ctx.Channel, Victim);
			if (r == null) {
				await ctx.RespondAsync("Failed, no response found.");
				return;
			}
			if(r.Content == "yes")
			{
				var cc = await FamilyManager.GetAsync(Victim.Id);
				if(cc != null)
				{
					ulong id = cc.PartnerId;
					
					await FamilyManager.RemoveAsync(id);
					await ctx.RespondAsync($"{Victim.Mention} divorced from <@{id}>");
				}
				var ns = new Family()
				{
					PID = Victim.Id,
					PartnerId = ctx.User.Id
				};
				var Nx = new Family()
				{
					PID = ctx.User.Id,
					PartnerId = Victim.Id
				};
				var c = await FamilyManager.GetAsync(ctx.User.Id);
				if(c != null)
				{
					ulong id = c.PartnerId;
					
					await FamilyManager.RemoveAsync(id);
					await ctx.RespondAsync($"{ctx.User.Mention} divorced from <@{id}>");
				}
				await FamilyManager.SyncAsync(ns);
				await FamilyManager.SyncAsync(Nx);
				await ctx.RespondAsync($"{ctx.User.Mention} and {Victim.Mention} have married!! <3");
			}
			else if(r.Content == "maybe")
			{
				await ctx.RespondAsync($"Try again next year.. Good luck my <@{ctx.User.Id}>");
			}
			else
			{
				await ctx.RespondAsync($"<@{ctx.User.Id}>.. M-maybe <@{Victim.Id}> just isn't the one..");
			}
		}
		[Command("show")]
		[Description("Shows mentioned user or self marriage status.")]
		public async Task ShowAsync(CommandContext ctx)
		{
			if(ctx.Message.MentionedUsers.Count > 0)
			{
				DiscordUser victim = ctx.Message.MentionedUsers.First();
				var cc = await FamilyManager.GetAsync(victim.Id);
				if(cc == null)
				{
					await ctx.RespondAsync($"<@{victim.Id}> isn't married.");
					return;
				}
				await ctx.RespondAsync($"<@{victim.Id}> is married to <@{cc.PartnerId}>");
			}
			else
			{
				var cc = await FamilyManager.GetAsync(ctx.User.Id);
				if(cc == null)
				{
					await ctx.RespondAsync("You aren't married.");
					return;
				}
				await ctx.RespondAsync($"<@{ctx.User.Id}> is married to <@{cc.PartnerId}>");
			}
		}
		[Command("divorce")]
		[Description("End your current marriage.")]
		public async Task DivorceAsync(CommandContext ctx)
		{
			var c = await FamilyManager.GetAsync(ctx.User.Id);
			if(c != null) 
			{
			var r = await Interactivity.WaitForAnswerAsync(ctx, $"Are you sure you want to divorce from <@{c.PartnerId}>?", 120000, ctx.Channel);
			if (r == null) {
				await ctx.RespondAsync("Failed, no response found.");
				return;
			}
			if(r.Content.ToLower() == "yes")
			{
				await FamilyManager.RemoveAsync(c.PartnerId);
				await FamilyManager.RemoveAsync(ctx.User.Id);
				await ctx.RespondAsync($"{ctx.User.Mention} divorced from <@{c.PartnerId}>");
			}
			else
			{
				await ctx.RespondAsync($"You still have some explaining to do.. HEY <@{c.PartnerId}> This bitch tryna cheat on you");
			}
			}
			else
			{
				await ctx.RespondAsync("You aren't married, but bro's before hoes! practising the divorce already hehe!!");
			}
		}
	}
}