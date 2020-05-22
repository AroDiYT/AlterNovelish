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
    [Group("Channel")]
    public class Channel : BaseCommandModule {
        [GroupCommand]
        public async Task ChannelAsync(CommandContext ctx, DiscordChannel ch = null)
        {
            ch = ch ?? ctx.Channel;
            var CH = await ManageCharacter.GetChannel(ch.Id);
            if(CH == null)
            {
                await ctx.RespondAsync("That channel hasn't been added to our DB yet");
                return;
            }
            string text = $"{ch.Mention}:\n**Category:** `{CH.Category}`";
            if(CH.Category == ChannelCategory.Rp)
            text += $"\n**Base-XP:** `{CH.XP}`";
            await ctx.RespondAsync(text);
        }
        [Command("set")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SetChannelAsync(CommandContext ctx, DiscordChannel ch = null)
        {
            ch = ch ?? ctx.Channel;
            async Task suicide(string why = "") {
            await ch.SendMessageAsync($"{why ?? ""} Stopping;");
            };
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`What Category does this channel belong to?`**", channel: ctx.Channel);
            if (r == null) {
                await suicide();
                return;
            }
            switch(r.Content.ToLower())
            {
                case "rp":
                    var sr = await Interactivity.WaitForAnswerINTAsync(ctx, $"**`What is the base XP gained from this channel?`**", channel: ctx.Channel);
                    await ManageCharacter.InsertChannel(ch.Id, ChannelCategory.Rp,sr);
                    await ctx.RespondAsync($"{ch.Mention} is now an `RP` channel;");
                return;
                case "chat":
                    await ManageCharacter.InsertChannel(ch.Id, ChannelCategory.Chat,0);
                    await ctx.RespondAsync($"{ch.Mention} is now an `Chat` channel;");
                return;
                case "bot":
                    await ManageCharacter.InsertChannel(ch.Id, ChannelCategory.Bot,0);
                    await ctx.RespondAsync($"{ch.Mention} is now an `Bot` channel;");
                return;
            }
        }
    }
}