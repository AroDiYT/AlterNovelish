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
    [Group("Inventory"), Aliases("inv")]
	[Description("WIP")]
	public class Inventory : BaseCommandModule {
        [GroupCommand()]
        public async Task View(CommandContext ctx, [RemainingText, Description("The text Novelish should say.")] String content)
        {
            
        }
    }
}
