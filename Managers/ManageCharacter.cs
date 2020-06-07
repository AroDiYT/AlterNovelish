using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Dapper;

using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Managers
{
    public static class ManageCharacter
    {
        public class Card
        {
            public int ChrEntry { get; set; }
            public int ChrCode { get; set; }
            public int ChrAmount { get; set; }
            public string ChrName { get; set; }
            public Race ChrRace { get; set; }
            public Classes ChrClass { get; set; }
            public string ChrImage { get; set; }
            public string ChrDesc { get; set; }
        }
        public class Issues
        {
            public int Entry { get; set; }
            public string Title { get; set; }
            public string Desc { get; set; }
            public Solved Solved { get; set; }
        }
        public class Suggestions
        {
            public int Entry { get; set; }
            public string Title { get; set; }
            public string Desc { get; set; }
            public Done Done { get; set; }
        }
        public enum ItemCategory
        {
            Weapon = 0,
            Food = 1,
            Armor = 2,
            Collectables = 3
        }
        public enum Solved
        {
            yes, no
        }
        public enum Done
        {
            yes, no
        }
        private static Database _db;

        /* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
        //public static ConcurrentDictionary<ulong, Character> Cache { get; private set; }
        public static ConcurrentDictionary<int, Chr> Cache { get; private set; }
        public static ConcurrentDictionary<(ulong UID, ulong GID), Account> CacheAccount { get; private set; }
        public static ConcurrentDictionary<(ulong UID, ulong GID, int Slot), Ownership> CacheOwnership { get; private set; }
        public static ConcurrentDictionary<(int InventoryID, int ItemID), Inventory> CacheInventory { get; set; }
        public static ConcurrentDictionary<int, Item> CacheItem { get; set; }

        /* Put initialization logic here */
        public static void Initialize(Database db)
        {
            _db = db;
            Cache = new ConcurrentDictionary<int, Chr>();
            CacheOwnership = new ConcurrentDictionary<(ulong UID, ulong GID, int Slot), Ownership>();
            CacheAccount = new ConcurrentDictionary<(ulong UID, ulong GID), Account>();
            CacheInventory = new ConcurrentDictionary<(int InventoryID, int ItemID), Inventory>();
            CacheItem = new ConcurrentDictionary<int, Item>();
            /*@Entry, @Name, @Gender, @Race, @Age, @Desc, @Image,
			@HPC, @HPM, @ENC, @ENM, @Class, @Sleight, @Marksman, @Dodge, @Strength,
			@Thought, @Speed, @Intellegence, @Magic, @MagicEff, @Level, @XP, @InventoryID, @SP*/
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Character (
			Entry INTEGER PRIMARY KEY,

			Name TEXT, Gender INT, Race INT, Age INT, Desc TEXT,
			Image TEXT,

			HPC INT, HPM INT,

			ENC INT, ENM INT,

			Class INT,

			Sleight INT, Marksman INT, Dodge INT,

			Strenght INT, Thought INT, Speed INT, Intellegence INT,

			Magic INT, MagicEff INT,

			Level INT, XP INT,

			InventoryID INTEGER, SP INT, IsAlter INT, Balance INT, Marriage BIGINT,

			SubClass INT, SubClassAttribute INT, UPoints INT, EffectPath INT, EffectAttribute INT, KID INT, PARENTA INT, PARENTB INT

            , Theme TEXT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Inventory (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			InventoryID INTEGER,
			ItemID INT,
			ItemAmount INT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Inventory_Customs (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			InventoryID INTEGER,
			Name TEXT, Description TEXT, Image TEXT, Attribute INT, Category INT, Effect INT
			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Equips (
				ChrEntry INT PRIMARY KEY,
				ItemID INT,
				IsCustom INT
			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Cards (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			ChrEntry INT,
			ChrCode INT,
			ChrAmount INT,
			ChrName TEXT,
			ChrRace INT,
			ChrClass INT,
			ChrDesc TEXT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Item (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			Name TEXT, Desc TEXT, Category INT, Attribute INT, Effect INT, Cost INT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Ownership (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			UID BIGINT, GID BIGINT, CharEntry INT, Slot INT, Remind INT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Account (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			UID BIGINT, GID BIGINT, Slot INT, Snaps INT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Channels (

			ID BIGINT PRIMARY KEY, CATEGORY INT, XP INT

			)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Issues (
				Entry INTEGER PRIMARY KEY,

				Title TEXT, Desc TEXT, Solved INT)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Suggestions (
				Entry INTEGER PRIMARY KEY,

				Title TEXT, Desc TEXT, Done INT)");
            _db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS UAbilities (

					Entry INTEGER PRIMARY KEY AUTOINCREMENT,

					Name TEXT, Description TEXT, Character INT, Attribute INT, Path INT

			)");
        }
        public static async Task RemoveAsync(Issues Issue)
        {
            var count = await ManageCharacter.GetCountAsync(Tables.Issues);
            await _db.DbConn.ExecuteAsync(@"DELETE FROM Issues WHERE Entry = @Entry", Issue);
            while (count > Issue.Entry)
            {
                var IssueGet = new ManageCharacter.Issues()
                {
                    Entry = count - 1
                };
                var Issues = await ManageCharacter.GetAll(IssueGet);
                if (Issues != null)
                {
                    Issues.Entry -= 1;
                    await ManageCharacter.UpdateAsync(Issue: Issues);
                }
                count--;
            }
        }
        public static async Task RemoveAsync(Suggestions Issue)
        {
            var count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Suggestions");
            await _db.DbConn.ExecuteAsync(@"DELETE FROM Suggestions WHERE Entry = @Entry", Issue);
            while (count > Issue.Entry)
            {
                var IssueGet = new ManageCharacter.Suggestions()
                {
                    Entry = count - 1
                };
                var Issues = await ManageCharacter.GetAlls(IssueGet);
                if (Issues != null)
                {
                    Issues.Entry -= 1;
                    await ManageCharacter.UpdateAsync(Suggest: Issues);
                }
                count--;
            }
        }
        public static async Task InsertChannel(ulong ID, ChannelCategory Cat, int XP)
        {
            await _db.DbConn.ExecuteAsync(@"INSERT INTO Channels VALUES (
				@ID, @Cat, @XP
				) ON CONFLICT(ID) DO UPDATE
				SET Category = @Cat, XP = @XP"
                , new { ID, Cat, XP });
        }
        public static async Task<Channel> GetChannel(ulong ID)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Channel>(@"SELECT * FROM Channels WHERE ID = @ID", new { ID });
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<bool> CheckName(string name)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE name Like @name", new { name = "%" + name + "%" });
            if (Chrs == null)
                return true;
            if (Chrs.Name.ToLower() == name.ToLower())
                return false;
            return true;
        }
        public static async Task InsertAsync(Chr Chr = null, Account Account = null, Ownership Ownership = null,
         Inventory Inv = null, Item Item = null, Card Card = null, Issues Issue = null, Suggestions Suggest = null,
         Custom Custom = null, Equiped Equip = null, UAbility Uniq = null)
        {
            if (Chr != null)
            {
                var Highest = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Character");
                Chr.InventoryID = Highest;
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Character VALUES (
			null, @Name, @Gender, @Race, @Age, @Desc, @Image,
			@HPC, @HPM, @ENC, @ENM, @Class, @Sleight, @Marksman, @Dodge, @Strenght,
			@Thought, @Speed, @Intellegence, @Magic, @MagicEff, @Level, @XP, @InventoryID, @SP, @IsAlter, @Balance, @Marriage,

			0,0,0,0,0,0,0,0,'none'
			)", Chr);
                Cache.TryAdd(Chr.Entry, Chr);
            }

            if (Inv != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Inventory VALUES (
			null, @InventoryID, @ItemID, @ItemAmount
			)", Inv);
                CacheInventory.TryAdd((Inv.InventoryID, Inv.ItemID), Inv);
            }

            if (Item != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Item VALUES (
			null, @Name, @Desc, @Category, @Attribute, @Effect, @Cost
			)", Item);
                CacheItem.TryAdd(Item.Entry, Item);
            }

            if (Ownership != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Ownership VALUES (
			null, @UID, @GID, @CharEntry, @Slot, 0
			)", Ownership);
                CacheOwnership.TryAdd((Ownership.UID, Ownership.GID, Ownership.Slot), Ownership);
            }

            if (Account != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Account VALUES (
			null, @UID, @GID, @Slot, 0
			)", Account);
                CacheAccount.TryAdd((Account.UID, Account.GID), Account);
            }

            if (Card != null)
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Cards VALUES (
				null, @ChrEntry, @ChrCode, @ChrAmount, @ChrName, @ChrRace, @ChrClass, @ChrDesc
			)", Card);

            if (Issue != null)
            {
                Issue.Entry = await ManageCharacter.GetCountAsync(Tables.Issues);
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Issues VALUES (
				@Entry, @Title, @Desc, 1
			)", Issue);
            }
            if (Suggest != null)
            {
                Suggest.Entry = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Suggestions");
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Suggestions VALUES (
				@Entry, @Title, @Desc, 1
			)", Suggest);
            }
            if (Custom != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Inventory_Customs VALUES (
					null, @InventoryID, @Name, @Description, @Image, @Attribute, @Category, @Effect
					)");
            }
            if (Equip != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT INTO Equips VALUES (
					@ChrEntry, @ItemID, @IsCustom
					) on conflict(ChrEntry) do update set ItemID = @ItemID, IsCustom = @IsCustom");
            }
            if (Uniq != null)
            {
                await _db.DbConn.ExecuteAsync(@"INSERT INTO UAbilities VALUES (
					null, @Name, @Description, @Character, @Attribute, @Path
					)", Uniq);
            }
        }
        public static async Task UpdateAsync(Chr Chr = null, Account Account = null, Ownership Ownership = null,
         Inventory Inventory = null, Item Item = null, Card Card = null, Issues Issue = null,
         Custom Custom = null, Equiped Equip = null, Suggestions Suggest = null, UAbility Uniq = null)
        {
            if (Chr != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Character SET
			Name = @Name, Gender = @Gender, Race = @Race, Age = @Age, Desc = @Desc, Image = @Image,
			HPC = @HPC, HPM = @HPM, ENC = @ENC, ENM = @ENM, Class = @Class, Sleight = @Sleight, Marksman = @Marksman, Dodge = @Dodge, Strenght = @Strenght,
			Thought = @Thought, Speed = @Speed, Intellegence = @Intellegence, Magic = @Magic, MagicEff = @MagicEff, Level = @Level, XP = @XP, SP = @SP, Balance = @Balance,
			Marriage = @Marriage, SubClass = @SubClass, SubClassAttribute = @SubClassAttribute, UPoints = @UPoints, EffectPath = @EffectPath, EffectAtribute = @EffectAtribute,
            KID = @KID, PARENTA = @PARENTA, PARENTB = @PARENTB, Theme = @Theme
			WHERE Entry = @Entry", Chr);
                ManageCharacter.UpdateCache(Chr);
            }

            if (Inventory != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Inventory SET
			ItemAmount = @ItemAmount
			WHERE InventoryID = @InventoryID AND ItemID = @ItemID", Inventory);
                ManageCharacter.UpdateCache(Inventory: Inventory);
            }

            if (Item != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Item
                SET
			Name = @Name, Desc = @Desc, Category = @Category, Attribute = @Attribute, Cost = @Cost
			WHERE Entry = @Entry", Item);
                ManageCharacter.UpdateCache(Item: Item);
            }

            if (Ownership != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Ownership
                SET
			Remind = @Remind
			WHERE CharEntry = @CharEntry", Ownership);
                ManageCharacter.UpdateCache(Ownership: Ownership);
            }
            if (Account != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Account SET
			Slot = @Slot, Snaps = @Snaps
			WHERE UID = @UID AND GID = @GID", Account);
                ManageCharacter.UpdateCache(Account: Account);
            }
            if (Card != null)
                await _db.DbConn.ExecuteAsync(@"UPDATE Cards
			SET
			ChrCode = @ChrCode, ChrAmount = @ChrAmount, ChrName = @ChrName, ChrRace = @ChrRace, ChrClass = @ChrClass, ChrDesc = @ChrDesc
			WHERE
			ChrEntry = @ChrEntry AND ChrCode = @ChrCode
			", Card);
            if (Issue != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Issues
			SET
			Title = @Title, Desc = @Desc, Solved = @Solved
			WHERE
			Entry = @Entry
			", Issue);
            }
            if (Suggest != null)
            {
                await _db.DbConn.ExecuteAsync(@"UPDATE Suggestions
			SET
			Title = @Title, Desc = @Desc, Done = @Done
			WHERE
			Entry = @Entry
			", Suggest);
            }
            if (Custom != null)
            {
                await _db.DbConn.ExecuteAsync(@"update Inventory_Customs SET
					Name = @Name, Description = @Description, Image = @Image, Attribute = @Attribute, Effect = @Effect
					Where Entry = @Entry AND InventoryID = @InventoryID");
            }
            if (Uniq != null)
            {
                await _db.DbConn.ExecuteAsync(@"
				UPDATE
					UAbilities
				SET
					Name = @Name, Description = @Description, Attribute = @Attribute, Path = @Path
				WHERE
					Character = @Character
				", Uniq);
            }

        }
        public static void UpdateCache(Chr Chr = null, Account Account = null, Ownership Ownership = null, Inventory Inventory = null, Item Item = null)
        {
            if (Chr != null)
            {
                Cache.TryRemove(Chr.Entry, out _);
                Cache.TryAdd(Chr.Entry, Chr);
            }

            if (Inventory != null)
            {
                CacheInventory.TryRemove((Inventory.InventoryID, Inventory.ItemID), out _);
                CacheInventory.TryAdd((Inventory.InventoryID, Inventory.ItemID), Inventory);
            }


            if (Item != null)
            {
                CacheItem.TryRemove(Item.Entry, out _);
                CacheItem.TryAdd(Item.Entry, Item);
            }


            if (Ownership != null)
            {
                CacheOwnership.TryRemove((Ownership.UID, Ownership.GID, Ownership.Slot), out _);
                CacheOwnership.TryAdd((Ownership.UID, Ownership.GID, Ownership.Slot), Ownership);
            }


            if (Account != null)
            {
                CacheAccount.TryRemove((Account.UID, Account.GID), out _);
                CacheAccount.TryAdd((Account.UID, Account.GID), Account);
            }
        }
        /*Here come the usefull functions.*/
        public static async Task<DiscordEmbed> EmbedProfileAsync(Chr Chr, CommandContext ctx, string Owner = null)
        {
            var Embed = new DiscordEmbedBuilder { };
            string Alter = "[Original]";
            if (Chr.IsAlter == IsAlter.Yes)
                Alter = "[Alter]";
            Embed = Embed.WithAuthor(Alter + $" {Chr.Name}'s profile", null, Chr.Image);
            string Gender = "Female";
            if (Chr.Gender == Objects.Sql.Profile.Gender.Male)
                Gender = "Male";
            var race = Chr.Race;

            var Class = Chr.Class;
            var Marriage = "Not Married";
            if (Chr.Marriage != 0)
            {
                var Chrg = new ChrGet()
                {
                    Entry = Chr.Marriage,
                };
                var Chrs = await ManageCharacter.GetAll(Chrg);
                Marriage = Chrs.Name;
            }
            string Child = "​";
            if(Chr.KID != 0)
            {
                var Chrg = new ChrGet()
                {
                    Entry = Chr.KID,
                };
                var Chrs = await ManageCharacter.GetAll(Chrg);
                Child = $"\n__**Kid:**__ `{Chrs.Name}`";
            }
            string Parent = "​";
            if(Chr.PARENTA != 0)
            {
                var Chrg = new ChrGet()
                {
                    Entry = Chr.PARENTA,
                };
                var Chrs = await ManageCharacter.GetAll(Chrg);
                Chrg = new ChrGet()
                {
                    Entry = Chr.PARENTB,
                };
                var Chrs2 = await ManageCharacter.GetAll(Chrg);
                Parent = $"\n __**Parents**__\n{Chrs.Name} and {Chrs2.Name}";
            }
			string Subc = "​";
			if(Chr.SubClass != 0)
			Subc = $"\n__**SubClass**__ `{Chr.SubClass}`";
            Embed = Embed.WithDescription($"__**Age**__ `{Chr.Age}`\n__**Gender**__ `{Gender}`\n__**Race**__ `{race}`\n__**Class**__ `{Class}`{Subc}\n\n__**Description**__ `{Chr.Desc}`\n\n __**Married:**__ `{Marriage}`{Child}{Parent}");
            int neededXP = Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level);
            var field = new StringBuilder();
            field.Append($"**Health** `{Chr.HPC}|{Chr.HPM}`\n**Energy** `{Chr.ENC}|{Chr.ENM}`\n\n");
            field.Append($"**RB**\n[`Sleight {Chr.Sleight}`] - [`Marksman {Chr.Marksman}`] - [`Dodge {Chr.Dodge}`]\n\n");
            field.Append($"**Stats:**\n ***Strength({Chr.Strenght}) - Thought({Chr.Thought}) - Speed({Chr.Speed}) - Intelligence({Chr.Intellegence})***\n\n");
            field.Append($"**Magic({Chr.Magic}) | Magic-Eff({Chr.MagicEff})**\n\n");
            field.Append($"**Level {Chr.Level}**\n__Progress__[{Chr.XP}|{neededXP}]\nSP: `{Chr.SP}`\nUPoints: `{Chr.UPoints}`\n\n" + Owner ?? "**Heaven X**");

            Embed = Embed.AddField("Stats/Skills", field.ToString());
            Embed = Embed.WithThumbnailUrl(Chr.Image);
            Embed = Embed.WithFooter(Chr.Theme);
            return Embed;
        }
        public static async Task<int> GetCountAsync(Tables table)
        {
            int count = 0;
            if (table == Tables.Character)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Character");
            if (table == Tables.Account)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Account");
            if (table == Tables.Ownership)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Ownership");
            if (table == Tables.Inventory)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Inventory");
            if (table == Tables.Item)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Item");
            if (table == Tables.Issues)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Issues");
            if (table == Tables.Suggest)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Suggestions");
            return count;
        }
        public static async Task<int> GetCountAsync(Tables table, OwnerGet Owner = null, InvGet Inv = null)
        {
            int count = 0;
            if (table == Tables.Ownership)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Ownership WHERE UID = @UID AND GID = @GID", Owner);
            if (table == Tables.Inventory)
                count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Inventory WHERE InventoryID = @InventoryID", Inv);
            return count;
        }
        public static async Task<Chr> GetAllCache(ChrGet Chr)
        {
            if (Cache.TryGetValue(Chr.Entry, out var chr))
            {
                return chr;
            }
            return null;
        }
        public static async Task<Account> GetAllCache(AccountGet Acc)
        {
            if (CacheAccount.TryGetValue((Acc.UID, Acc.GID), out var acc))
            {
                return acc;
            }
            return null;
        }
        public static async Task<Ownership> GetAllCache(OwnerGet Owner)
        {
            if (CacheOwnership.TryGetValue((Owner.UID, Owner.GID, Owner.Slot), out var Own))
            {
                return Own;
            }
            return null;
        }
        public static async Task<Inventory> GetAllCache(InvGet Inv)
        {
            if (CacheInventory.TryGetValue((Inv.InventoryID, Inv.ItemID), out var INV))
            {
                return INV;
            }
            return null;
        }
        public static async Task<Item> GetAllCache(ItemGet Item)
        {

            if (CacheItem.TryGetValue(Item.ItemID, out var Items))
            {
                return Items;
            }
            return null;
        }
        public static async Task<Chr> GetChrByName(string name)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE name Like @name", new { name = "%" + name + "%" });
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Ownership> GetOwnerByEntry(int Entry)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Ownership>(@"SELECT * FROM Ownership WHERE CharEntry = @Entry", new { Entry });
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Chr> GetAll(ChrGet Chr)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE Entry = @Entry", Chr);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Account> GetAll(AccountGet Acc)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Account>(@"SELECT * FROM Account WHERE UID = @UID AND GID = @GID", Acc);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Ownership> GetAll(OwnerGet Owner)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Ownership>(@"SELECT * FROM Ownership WHERE UID = @UID AND GID = @GID AND Slot = @Slot", Owner);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Inventory> GetAll(InvGet Inv)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Inventory>(@"SELECT * FROM Inventory WHERE InventoryID = @InventoryID AND ItemID = @ItemID", Inv);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Item> GetAll(ItemGet Item)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Item>(@"SELECT * FROM Item WHERE ItemID = @ItemID", Item);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Card> GetAll(Card Card)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Card>(@"SELECT * FROM Cards WHERE ChrEntry = @ChrEntry AND ChrCode = @ChrCode", Card);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Issues> GetAll(Issues Issue)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Issues>(@"SELECT * FROM Issues WHERE Entry = @Entry", Issue);
            if (Chrs != null)
                return Chrs;
            return null;
        }
        public static async Task<Suggestions> GetAlls(Suggestions Issue)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Suggestions>(@"SELECT * FROM Suggestions WHERE Entry = @Entry", Issue);
            if (Chrs != null)
                return Chrs;
            return null;
        }
		public static async Task<UAbility> GetAll(UAbility Uniq)
        {
            var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<UAbility>(@"SELECT * FROM UAbilities WHERE Character = @Character", Uniq);
            if (Chrs != null)
                return Chrs;
            return null;
        }
    }
}
