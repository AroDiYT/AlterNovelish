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
    [Group("Item")]
	[Description("WIP")]
	public class Item : BaseCommandModule {
        [GroupCommand()]
        public async Task View(CommandContext ctx, [RemainingText, Description("The text Novelish should say.")] String content)
        {
            
        }
        [Command("new")]
        public async Task New(CommandContext ctx)
        {
            
        }
    }
}
