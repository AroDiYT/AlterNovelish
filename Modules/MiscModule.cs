using System;
using System.Threading.Tasks;
using System.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Diagnostics;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql;

namespace BotTemplate.Modules {
	public class MiscModule : BaseCommandModule {
      [Command("ping")]
        [Description("Showing information about the bot.")]
         public async Task PingAsync(CommandContext ctx)
        {
            var sw = Stopwatch.StartNew();
            var message = await ctx.RespondAsync("pinging"); // this sends a message
            sw.Stop();

            await message.ModifyAsync($"ping: {sw.ElapsedMilliseconds} ms"); // edit the message
        }
      [Command("chaos")]
      [Cooldown(1, 10, CooldownBucketType.Global)]
		  public async Task ChaosAsync(CommandContext ctx, [RemainingText] string Target) {
        var chaos = ctx.Guild.Members.FirstOrDefault(kv => kv.Value.Username == "Chaosbringer")
        .Value;

        var num = Helpers.Tools.RNG.Next(7) + 1;
        switch (num)
        {
          case 1:
          await ctx.RespondAsync(chaos.Mention + " is a cute little cloud of chaos <3");
          break;
          case 2:
          await ctx.RespondAsync($"{chaos.Mention}, You are a nice little Chaos");
          break;
          case 3:
          await ctx.RespondAsync($"{chaos.Mention}, Dash says you are the best");
          break;
          case 4:
          await ctx.RespondAsync($"{chaos.Mention} wants to marry {Target ?? "Dash"}");
          break;
          case 5:
          await ctx.RespondAsync("THIS IS AN S.O.S. FROM DASH");
          break;
          case 6:
          await ctx.RespondAsync("You have escaped a ping but you are still a nice little Chaos");
          break;
          case 7:
          await ctx.RespondAsync($"{chaos.Mention} You are being summoned.");
          break;
        }    
		  }
      [Command("summon")]
      public async Task AnimalAsync(CommandContext ctx){
        var ani = Helpers.Tools.RNG.Next(13) + 1;
        switch (ani)
        {
          case 1:
          await ctx.RespondAsync(":service_dog:");
          break;
          case 2:
          await ctx.RespondAsync(":cat2:");
          break;
          case 3:
          await ctx.RespondAsync(":rabbit2:");
          break;
          case 4:
          await ctx.RespondAsync(":mouse2:");
          break;
          case 5:
          await ctx.RespondAsync(":peacock:");
          break;
          case 6:
          await ctx.RespondAsync(":sheep:");
          break;
          case 7:
          await ctx.RespondAsync(":unicorn:");
          break;
          case 8:
          await ctx.RespondAsync(":owl:");
          break;
          case 9:
          await ctx.RespondAsync(":pig2:");
          break;
          case 10:
          await ctx.RespondAsync(":giraffe:");
          break;
          case 11:
          await ctx.RespondAsync(":rooster:");
          break;
          case 12:
          await ctx.RespondAsync(":chipmunk:"); 
          break;
          case 13: 
          await ctx.RespondAsync(":butterfly:");
          break;
        }
      }
      [Command("Who")]
      public async Task WhoAsync(CommandContext ctx)
      {
        await ctx.RespondAsync($"{ctx.Guild.Members.Values.ToArray()[Helpers.Tools.RNG.Next(ctx.Guild.Members.Count)].Mention}");
      }

	  }
}
