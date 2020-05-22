using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules {
  
    [Group("Character"), Aliases(new string[] {"ch","char"})]
	  [Description("The Character Group")]
	public class Character : BaseCommandModule {
        private Random _rng = new Random();
        [GroupCommand()]
        public async Task ViewAsync(CommandContext ctx)
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
         
          var emmy = await ManageCharacter.EmbedProfileAsync(Chr);
          await ctx.RespondAsync(embed : emmy);
        }
        [Command("search")]
        public async Task SearchAsync(CommandContext ctx, [RemainingText] string name)
        {
          var Chr = await ManageCharacter.GetChrByName(name);
          if(Chr == null)
          {
            await ctx.RespondAsync("No character by that name.");
            return;
          }
          string Owner = "";
          var Own = await ManageCharacter.GetOwnerByEntry(Chr.Entry);
          if(Own == null)
          {
            Owner = "**From: No one**";
          }
          else
          {
            Owner = "**From: <@" + Own.UID + ">**";
          }
          var emmy = await ManageCharacter.EmbedProfileAsync(Chr, Owner);
          await ctx.RespondAsync(embed : emmy);
        }
        [Command("To")]
        public async Task SlotAsync(CommandContext ctx, int Slot)
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
            Slot = Slot
          };
          var Own = await ManageCharacter.GetAll(OwnGet);
          if(Own == null)
          {
            await ctx.RespondAsync("You don't have a character on that slot.");
            return;
          }
          Check.Slot = Slot;
          await ManageCharacter.UpdateAsync(Account : Check);
          await ctx.RespondAsync($"**Switched to slot** {Slot}");
        }
        [Command("To")]
        public async Task NameAsync(CommandContext ctx, [RemainingText] string name)
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

          var Slot = await ManageCharacter.GetChrByName(name);
          if(Slot == null)
          {
            await ctx.RespondAsync("No character by that name.");
            return;
          }
          
          var Own = await ManageCharacter.GetOwnerByEntry(Slot.Entry);
          if(Own == null)
          {
            await ctx.RespondAsync("No one owns this character.");
            return;
          }

          if(Own.UID != ctx.User.Id)
          {
            await ctx.RespondAsync("You do not own this character.");
            return;
          }

          if(Own.GID != ctx.Guild.Id)
          {
            await ctx.RespondAsync("This character currently doesn't belong to this guild.");
            return;
          }

          Check.Slot = Own.Slot;
          await ManageCharacter.UpdateAsync(Account: Check);

          await ctx.RespondAsync($"**Switched to ** {Slot.Name}");
        }
        [Command("List"),Aliases("ls")]
        public async Task ListAsync(CommandContext ctx)
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
          int i = 0;
          var sb = new StringBuilder();
          while(i < 16)
          {
            var OwnGet = new OwnerGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            Slot = i
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            if(Own == null)
             {
                i += 100;
             }
            else
            {
              var ChrGet = new ChrGet() {
                Entry = Own.CharEntry,
              };
              var Chr = await ManageCharacter.GetAll(ChrGet);
              sb.Append($"`[{i}]` **→** **[__{Chr.Class}__ - __{Chr.Name}__ - __{Chr.Race}__]**\n");
              i++;
            }
          }
          var Embed = new DiscordEmbedBuilder {
                Description = sb.ToString()
               };
          await ctx.RespondAsync(embed : Embed);
        }
        [Command("Lvlup")]
        public async Task LevelUpAsync(CommandContext ctx)
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
         
          var emmy = await ManageCharacter.EmbedProfileAsync(Chr);
          var SPS = Chr.SP;
          if(SPS == 0)
          goto END;
           while(SPS > 0)
          {  
            string stats = "`[HP]-[Energy]-[Sleight]-[Marksman]-[Dodge]-[Strength]-[Thought]-[Speed]-[Intellegence]-[Magic]-[MagicEff] - [:save]`";
            string Q2 = $"Where do you want to spend your `{SPS}` Statpoints\n\n{stats}";
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ctx.Channel);
            var ch = ctx.Channel;
            string St = $"**How much do you want to spend in {r.Content.ToUpper()}?**";
            switch(r.Content.ToLower())
            {
              case "hp":
                var res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.HPM += res*10;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "energy":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.ENM += res*10;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "sleight":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < SPS && res > -1) {
                  SPS -= res;
                  Chr.Sleight += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "marksman":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Marksman += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "dodge":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Dodge += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "strength":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Strenght += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "thought":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Thought += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "speed":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Speed += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "intellegence":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Intellegence += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "magic":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Magic += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "magiceff":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.MagicEff += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case ":save":
              goto END;
              default:
              await r.RespondAsync("That is not one of our stats.");
              break;
            }
          }
          END:
          Chr.SP = SPS;
          await ManageCharacter.UpdateAsync(Chr);
          await ctx.RespondAsync("Done;");
        }
        [Command("Edit")]
        public async Task EditAsync(CommandContext ctx)
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

          var emmy = await ManageCharacter.EmbedProfileAsync(Chr);
          for(;;)
          {  
            string stats = "`[Name]-[Age]-[Desc]-[Image]-[:save]`";
            string Q2 = $"What do you want to edit?\n\n{stats}";
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ctx.Channel);
            var ch = ctx.Channel;
            async Task suicide(string why = "") {
            await ch.SendMessageAsync($"{why ?? ""} Stopping Edit.");
            };
            string St = $"**Enter a value for this field.**";
            switch(r.Content.ToLower())
            {
              case "name":
                var res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                if(await ManageCharacter.CheckName(res.Content))
                {
                Chr.Name = res.Content;
                }
                else
                {
                  await res.RespondAsync("That name is already used by someone else!");
                }
              break;
              case "age":
                var rs = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                Chr.Age = rs;
              break;
              case "desc":
                res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                Chr.Desc = res.Content;
              break;
              case "image":
                res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                r = res;
                if (string.IsNullOrEmpty(r.Content)) {
                if (r.Attachments.Count == 0) {
                  await suicide("No image given.");
                  return;
                }
                        using (WebClient webClient = new WebClient()) 
                         {
                        webClient.DownloadFile(r.Attachments.FirstOrDefault().Url, "./image.png");
                        DiscordChannel ccs = await ctx.Client.GetChannelAsync(705752802806202427);
                        DiscordMessage ms = await ccs.SendFileAsync("./image.png", ctx.User.Username, false, null, null);
                        Chr.Image = ms.Attachments.FirstOrDefault().Url;
                          }
                } else if (r.Content.ToLowerInvariant() != "none") {
                  if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
                   Chr.Image = r.Content;
                 } else {
                    await suicide("Invalid Uri.");
                    return;
                 }
                } else {
                  Chr.Image = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
                }
              break;
              case ":save":
              goto END;
              default:
              await r.RespondAsync("That is not one of our fields.");
              break;
            }
          }
          END:
          await ManageCharacter.UpdateAsync(Chr);
          await ctx.RespondAsync("Done;");
        }
        [Command("new")]
        [Description("Create a new Character.")]
        public async Task NewAsync(CommandContext ctx)
        {
          var Accg = new AccountGet() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id
          };
          var Check = await ManageCharacter.GetAll(Acc: Accg);
          var Acc = new Account();
          if(Check == null)
          {
            Acc.Slot = 0;
            Acc.GID = ctx.Guild.Id;
            Acc.UID = ctx.User.Id;
            await ManageCharacter.InsertAsync(Account: Acc);
          }
          else
          {
            Acc = Check;
            var Own = new OwnerGet() {
              UID = ctx.User.Id,
              GID = ctx.Guild.Id
            };
            int slot = await ManageCharacter.GetCountAsync(Tables.Ownership, Owner: Own);
            if(slot > 15)
            {
              await ctx.RespondAsync("You already have 15 Characters.");
              return;
            }
            Acc.Slot = slot;
            await ManageCharacter.UpdateAsync(Account: Acc);
          }
          var overwrites = new DiscordOverwriteBuilder[] {
            new DiscordOverwriteBuilder().Allow(Permissions.AccessChannels).For(ctx.Member),
            new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(ctx.Guild.EveryoneRole)
          };

          var ch = await ctx.Guild.CreateTextChannelAsync($"{ctx.User.Username}{ctx.User.Discriminator}-setup",
              overwrites: overwrites);
                await ch.SendMessageAsync($"{ctx.User.Mention}, here you will be creating an new character.\n\nRaces: Human, Cyborg, Dragonoid, Fairy, Kitsune, Elf, Spirit, Ghost, Celeste, Neko\nClasses: Archer, Warrior, Mage, Thief, Defender");
                async Task suicide(string why = "") {
            await ch.SendMessageAsync($"{why ?? ""} Stopping setup.");
            await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
            await Task.Delay(10 * 1000);
            await ch.DeleteAsync();
          };
          int Question = 8;
          /*@Entry, @Name, @Gender, @Race, @Age, @Desc, @Image,
            @HPC, @HPM, @ENC, @ENM, @Class, @Sleight, @Marksman, @Dodge, @Strength,
            @Thought, @Speed, @Intellegence, @Magic, @MagicEff, @Level, @XP, @InventoryID, @SP*/
          var Chr = new Chr(){
            Name = "null",
            Age = 0,
            Gender = Gender.Male,
            Race = Race.Human,
            Desc = "null",
            Image = "null",

            Entry = 0,
            InventoryID = 0,

            HPC = 100,
            HPM = 100,
            ENC = 100,
            ENM = 100,

            Class = Classes.Warrior,

            Sleight = 0,
            Speed = 0,
            Marksman = 0,
            Dodge = 0,
            Strenght = 0,
            Thought = 0,
            Intellegence = 0,

            Magic = 0,
            MagicEff = 0,

            SP = 0,

            Level = 1,
            XP = 0,

            IsAlter = IsAlter.No,
            Balance = 100
          };
          var Questions = new string[] {"Image","Description","State, Alter or not? (answer with YES or NO)","Race","Class","Gender","Age","Name"};
          while(Question > 0)
          {
            QUESTION:
            int Ques = Question - 1;
            string Q = $"Give us your {Questions[Ques]}.";
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ch);
            if (r == null) {
            await suicide();
            return;
            }

            switch(Questions[Ques])
            {
              case "Name":
              if(await ManageCharacter.CheckName(r.Content))
              {
              Chr.Name = r.Content;
              Question -= 1;
              }
              else
              {
                await r.RespondAsync("That name is already in use!");
              }
              break;
              case "Age":
                if (int.TryParse(r.Content, out var res)){
                  Chr.Age = res;
                }
                else
                {
                  Chr.Age = 0;
                }
                Question -= 1;
              break;
              case "Gender":
                switch(r.Content.ToLower())
                {
                  case "male":
                  Chr.Gender = Gender.Male;
                  break;
                  case "female":
                  Chr.Gender = Gender.Female;
                  break;
                  default:
                  var c = _rng.Next(1);
                  if(c == 0)
                  Chr.Gender = Gender.Female;
                  if(c == 1)
                  Chr.Gender = Gender.Male;
                  break;
                }
                Question -= 1;
              break;
              case "Race":
                switch(r.Content.ToLower()) {
                  case "human":
                  Chr.Race = Race.Human;
                  Chr.Intellegence += 4;
                  Chr.Marksman += 2;
                  Chr.Strenght += 2;
                  break;
                  case "ghost":
                  Chr.Race = Race.Ghost;
                  Chr.MagicEff += 5;
                  Chr.Speed += 3;
                  Chr.Magic += 1;
                  break;
                  case  "neko":
                  Chr.Race = Race.Neko;
                  Chr.Speed += 5;
                  Chr.Magic += 2;
                  Chr.MagicEff += 2; 
                  break;
                  case "spirit":
                  Chr.Race = Race.Spirit;
                  Chr.Magic += 4;
                  Chr.MagicEff += 3;
                  Chr.ENM += 20;
                  break;
                  case "fairy":
                  Chr.Race = Race.Fairy;
                  Chr.Magic += 4;
                  Chr.MagicEff += 4;
                  Chr.HPM -= 10;
                  Chr.Thought += 2;
                  break;
                  case "elf":
                  Chr.Race = Race.Elf;
                  Chr.Magic += 4;
                  Chr.MagicEff += 4;
                  Chr.HPM -= 10;
                  Chr.Intellegence += 2;
                  break;
                  case "cyborg":
                  Chr.Race = Race.Cyborg;
                  Chr.HPM += 20;
                  Chr.Intellegence += 4;
                  Chr.Strenght += 3;
                  break;
                  case "celeste":
                  Chr.Race = Race.Celeste;
                  Chr.HPM += 60;
                  Chr.Dodge += 3; 
                  break;
                  case "kitsune":
                  Chr.Race = Race.Kitsune;
                  Chr.Intellegence += 3;
                  Chr.Magic += 3;
                  Chr.MagicEff += 3;
                  break;
                  case "dragonoid":
                    Chr.Race = Race.Dragonoid;
                    Chr.Strenght += 3;
                    Chr.Magic += 1;
                    Chr.Marksman += 3;
                    Chr.Intellegence += 1;
                    Chr.Thought += 1;
                  break;
                  default:
                  await ch.SendMessageAsync("Invalid race");
                  goto QUESTION;
                }
                Question -= 1;
              break;
              case "Class":
                switch(r.Content.ToLower()) {
                  case "warrior":
                  Chr.Class = Classes.Warrior;
                  Chr.Sleight += 6;
                  break;
                  case "archer":
                  Chr.Class = Classes.Archer;
                  Chr.Marksman += 6;
                  break;
                  case  "mage":
                  Chr.Class = Classes.Mage;
                  Chr.MagicEff += 6;
                  break;
                  case "thief":
                    Chr.Class = Classes.Thief;
                    Chr.Dodge += 6;
                  break;
                  case "defender":
                    Chr.Class = Classes.Defender;
                    Chr.HPM += 30;
                    Chr.ENM += 30;
                  break;
                  default:
                  await ch.SendMessageAsync("Invalid Class");
                  goto QUESTION;
                }
                Question -= 1;
              break;
              case "Description":
              Chr.Desc = r.Content;
              Question -= 1;
              break;
              case "State, Alter or not? (answer with YES or NO)":
                if(r.Content.StartsWith("ye"))
                  Chr.IsAlter = IsAlter.Yes;
                Question -= 1;
              break;
              case "Image":
                if (string.IsNullOrEmpty(r.Content)) {
                if (r.Attachments.Count == 0) {
                  await suicide("No image given.");
                  return;
                }
                        using (WebClient webClient = new WebClient()) 
                         {
                        webClient.DownloadFile(r.Attachments.FirstOrDefault().Url, "./image.png");
                        DiscordChannel ccs = await ctx.Client.GetChannelAsync(705752802806202427);
                        DiscordMessage ms = await ccs.SendFileAsync("./image.png", ctx.User.Username, false, null, null);
                        Chr.Image = ms.Attachments.FirstOrDefault().Url;
                          }
                } else if (r.Content.ToLowerInvariant() != "none") {
                  if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
                   Chr.Image = r.Content;
                 } else {
                    await suicide("Invalid Uri.");
                    return;
                 }
                } else {
                  Chr.Image = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
                }
                Question -= 1;
              break;
            }

          }
          int SPS = 5;
          
          while(SPS > 0)
          {  
            string stats = "`[HP]-[Energy]-[Sleight]-[Marksman]-[Dodge]-[Strength]-[Thought]-[Speed]-[Intellegence]-[Magic]-[MagicEff]`";
            string Q2 = $"Where do you want to spend your `{SPS}` Statpoints\n\n{stats}";
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ch);
            string St = $"**How much do you want to spend in {r.Content.ToUpper()}?**";
            switch(r.Content.ToLower())
            {
              case "hp":
                var res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.HPM += res*10;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "energy":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.ENM += res*10;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "sleight":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < SPS && res > -1) {
                  SPS -= res;
                  Chr.Sleight += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "marksman":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Marksman += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "dodge":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Dodge += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "strength":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Strenght += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "thought":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Thought += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "speed":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Speed += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "intellegence":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Intellegence += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "magic":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.Magic += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              case "magiceff":
                res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                if(res < (SPS + 1) && res > -1) {
                  SPS -= res;
                  Chr.MagicEff += res;
                }
                else
                {
                  await r.RespondAsync("You don't have that much SP");
                }
              break;
              default:
              await r.RespondAsync("That is not one of our stats.");
              break;
            }
          }
          int result = await ManageCharacter.GetCountAsync(Tables.Character);
          Chr.InventoryID = result;
          Chr.Entry = result;


          var Ownr = new Ownership() {
            UID = ctx.User.Id,
            GID = ctx.Guild.Id,
            CharEntry = result,
            Slot = Acc.Slot
          };
          
          await ManageCharacter.InsertAsync(Chr, Ownership: Ownr);
          await ctx.RespondAsync("Done;");
          await suicide("You've finished setup, deleting channel soon.");
        }
    }
}
		