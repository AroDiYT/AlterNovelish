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
    [Group("note"), Aliases("notes")]
	[Description("View notes.")]
	public class NoteModule : BaseCommandModule {
		[GroupCommand]
        public async Task NoteViewAsync(CommandContext ctx, [RemainingText] string name) {
			var note = await NoteManager.GetAsync(name);
			if (note == null)
				throw new UserException($"No such note: {name}");

			await ctx.RespondAsync($"\"{note.Value}\"\n by <@{note.Author}>");
		}
        [Command("new")]
		[RequirePermissions(Permissions.ManageMessages)]
        [Description("Create a new note.")]
		public async Task NewNoteAsync(CommandContext ctx) {
			var note = new Note();
			DiscordChannel ch = ctx.Channel;
			async Task suicide(string why = "") {
				await ch.SendMessageAsync($"{why ?? ""} Stopping setup.");
			};
			var r = await Interactivity.WaitForAnswerAsync(ctx, "What is the name of this note?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
			if (await NoteManager.GetAsync(r.Content) != null) /* already exists */
				throw new UserException("you already have a note with this name");
			note.Name = r.Content;
			r = await Interactivity.WaitForAnswerAsync(ctx, "What should be in the note?", channel: ch);
			if (r == null) {
				await suicide();
				return;
			}
			note.Value = r.Content;
			note.Author = ctx.User.Id;
			await NoteManager.SyncAsync(note);
			await ctx.RespondAsync($"Done. view with `&note {note.Name}`");
		}
    }
}
		