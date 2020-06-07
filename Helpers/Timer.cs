using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Dapper;

using BotTemplate.Objects.Sql.Profile;
using BotTemplate.Managers;

namespace BotTemplate.Helpers
{
    public class Time
    {
        public int CharacterID { get; set; }
        public int RemainingTime { get; set; }
        public string Command { get; set; }
    }
    public static class TimerChr
    {
        private static System.Timers.Timer aTimer;
        private static Database _db;
        public static ConcurrentDictionary<(int ID, string CMD), Time> Cooldowns { get; private set; }
        public static void Init(Database db)
        {
            Cooldowns = new ConcurrentDictionary<(int ID, string CMD), Time>();
            _db = db;
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Cooldowns (
                    Entry INTEGER PRIMARY KEY,
                    CharacterID INT, Command TEXT, RemainingTime INT
                )");
            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 1000;

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += AddSecondCooldown;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }
        public static async Task<Time> GetCooldown(int ID, string CMD)
        {
            if (Cooldowns.TryGetValue((ID, CMD), out var Res))
                return Res;
            return null;
        }
        public static async Task LoadCooldown()
        {
            foreach (var i in await _db.DbConn.QueryAsync<Time>("SELECT * FROM Cooldowns"))
            {
                Cooldowns.TryAdd((i.CharacterID, i.Command), i);
            }
            await _db.DbConn.ExecuteAsync(@"DELETE FROM Cooldowns;");
        }
        public static async Task SaveCooldown()
        {
            int i = 0;
            foreach (var cooldown in Cooldowns.Values)
            { // Only gets Time

                var Chr = cooldown.CharacterID;
                var Cmd = cooldown.Command;
                var RemainingTime = cooldown.RemainingTime;

                await _db.DbConn.ExecuteAsync(@"insert into Cooldowns Values(
                    @i, @Chr, @Cmd, @RemainingTime
                )", new { i, Chr, Cmd, RemainingTime });

                i++;
            }
        }
        public static async Task AddCooldown(int Chr, string Cmd, int Time)
        {
            if(Cooldowns.TryGetValue((Chr, Cmd),out var res))
            {
                Cooldowns.Remove((Chr, Cmd), out _);
            }
            var TimeTable = new Time(){
                CharacterID = Chr,
                Command = Cmd,
                RemainingTime = Time
            };

            Cooldowns.TryAdd((Chr, Cmd), TimeTable);
        }
        public static async void AddSecondCooldown(object sender, EventArgs e)
        {
            foreach (var cooldown in Cooldowns.Values)
            { // Only gets Time
                cooldown.RemainingTime -= 1;
                Cooldowns.TryRemove((cooldown.CharacterID, cooldown.Command), out _);
                if(cooldown.RemainingTime > 0)
                Cooldowns.TryAdd((cooldown.CharacterID, cooldown.Command), cooldown);
                if(cooldown.RemainingTime == 0)
                {
                  var Owner = await ManageCharacter.GetOwnerByEntry(cooldown.CharacterID);
                  if(Owner.Remind == 0)
                  {
                    var Ccr = await ManageCharacter.GetAll(new ChrGet(){
                        Entry = Owner.CharEntry
                    });
                    var c = Bot.client2;
              			var all = c.Guilds;
              			foreach(var es in all)
              					{
                                try
                                    {
                                        DiscordGuild gs = es.Value;
                                        if(gs.GetMemberAsync(Owner.UID) != null)
                                        {
                                            var mem = await gs.GetMemberAsync(Owner.UID);
                                            try
                                            {
                                                await mem.SendMessageAsync($"`Character: {Ccr.Name} || Command: {cooldown.Command}` is ready to use.");
                                            return;
                                            }
                                            catch{
                                                Console.WriteLine("Not Dmable Person Found, Please slap them!");
                                            }
                                            
                                        }
                                    } catch {
                                        return;
                                    }
              					}
                  }
                }
            }
        }
    }
    public class TimerUser
    {

    }
    public class TimerChannel
    {

    }
}
