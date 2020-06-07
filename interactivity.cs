using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;

using BotTemplate;

namespace BotTemplate {
	public static class Interactivity {

		/// <summary>
		///   Wait for a message while ignoring any messages starting with ';'
		/// </summary>
		public static async Task<DiscordMessage> WaitForAnswerAsync(CommandContext ctx, string query,
				uint delay = 1000 * 120, DiscordChannel channel = null,
				DiscordUser who = null) {
			var inter = ctx.Client.GetInteractivity();
			channel = channel ?? ctx.Channel;
			who = who ?? ctx.User;

			await channel.SendMessageAsync(query);

			var result = await inter.WaitForMessageAsync(m => m.Channel == channel && m.Author == who
														 && !m.Content.StartsWith(';'));
			if (result.TimedOut) {
				await channel.SendMessageAsync("Timed out.");
				return null;
			}

			return result.Result;
		}

		public static async Task<DiscordMessage> WaitForAnswerAsync(CommandContext ctx, DiscordEmbed embed,
				uint delay = 1000 * 120, DiscordChannel channel = null,
				DiscordUser who = null) {
			var inter = ctx.Client.GetInteractivity();
			channel = channel ?? ctx.Channel;
			who = who ?? ctx.User;

			await channel.SendMessageAsync(embed: embed);

			var result = await inter.WaitForMessageAsync(m => m.Channel == channel && m.Author == who
														 && !m.Content.StartsWith(';'));
			if (result.TimedOut) {
				await channel.SendMessageAsync("Timed out.");
				return null;
			}

			return result.Result;
		}
		public static async Task<int> WaitForAnswerINTAsync(CommandContext ctx, string query,
				uint delay = 1000 * 120, DiscordChannel channel = null,
				DiscordUser who = null) {
			var inter = ctx.Client.GetInteractivity();
			channel = channel ?? ctx.Channel;
			who = who ?? ctx.User;

			await channel.SendMessageAsync(query);
			int res;
			for (;;) {
			var r = await inter.WaitForMessageAsync(m => m.Channel == channel && m.Author == who
														 && !m.Content.StartsWith(';'));
			if (r.TimedOut) {
				await channel.SendMessageAsync("Timed out.");
				return 0;
			}
			if (int.TryParse(r.Result.Content, out res) && res > -1)
				return res;
			}
		}
		public static async Task<DiscordUser> WaitForStealAsync(CommandContext ctx,
				uint delay = 1000 * 120, DiscordChannel channel = null) {
			var inter = ctx.Client.GetInteractivity();
			channel = channel ?? ctx.Channel;

			var result = await inter.WaitForMessageAsync(m => m.Channel == channel
														 && m.Content.StartsWith(".claim") && !m.Author.IsBot);
			if (result.TimedOut) {
				await channel.SendMessageAsync("Timed out.");
				return null;
			}

			return result.Result.Author;
		}
	}
}
