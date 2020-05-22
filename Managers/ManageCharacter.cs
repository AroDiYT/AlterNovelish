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

namespace BotTemplate.Managers {
	public static class ManageCharacter {
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
		public static void Initialize(Database db) {
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

			InventoryID INTEGER, SP INT, IsAlter INT, Balance INT

			)");
			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Inventory (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			InventoryID INTEGER,
			ItemID INT,
			ItemAmount INT

			)");
			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Item (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			Name TEXT, Desc TEXT, Category INT, Attribute INT, Cost INT

			)");
			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Ownership (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			UID BIGINT, GID BIGINT, CharEntry INT, Slot INT

			)");
			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Account (
			Entry INTEGER PRIMARY KEY AUTOINCREMENT,

			UID BIGINT, GID BIGINT, Slot INT

			)");
			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Channels (

			ID BIGINT PRIMARY KEY, CATEGORY INT, XP INT

			)");
		}
		public static async Task InsertChannel(ulong ID, ChannelCategory Cat, int XP)
		{
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Channels VALUES (
				@ID, @Cat, @XP
				) ON CONFLICT(ID) DO UPDATE
				SET Category = @Cat, XP = @XP"
				, new {ID, Cat, XP});
		}
		public static async Task<Channel> GetChannel(ulong ID)
		{
			var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Channel>(@"SELECT * FROM Channels WHERE ID = @ID", new {ID});
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<bool> CheckName(string name)
		{
			var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE name Like @name", new {name = "%" + name + "%"});
			if(Chrs == null)
			return true;
			if(Chrs.Name.ToLower() == name.ToLower())
			return false;
			return true;
		}
		public static async Task InsertAsync(Chr Chr = null, Account Account = null,Ownership Ownership = null, Inventory Inv = null, Item Item = null)
		{
			if(Chr != null) {
			var Highest = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Character");
			Chr.InventoryID = Highest;
			await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Character VALUES (
			null, @Name, @Gender, @Race, @Age, @Desc, @Image,
			@HPC, @HPM, @ENC, @ENM, @Class, @Sleight, @Marksman, @Dodge, @Strenght,
			@Thought, @Speed, @Intellegence, @Magic, @MagicEff, @Level, @XP, @InventoryID, @SP, @IsAlter, @Balance
			)",Chr);
			Cache.TryAdd(Chr.Entry, Chr);
			}
			
			if(Inv != null) {
			await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Inventory VALUES (
			null, @InventoryID, @ItemID, @ItemAmount
			)", Inv);
			CacheInventory.TryAdd((Inv.InventoryID,Inv.ItemID), Inv);
			}

			if(Item != null) {
			await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Item VALUES (
			null, @Name, @Desc, @Category, @Attribute, @Cost
			)", Item);
			CacheItem.TryAdd(Item.Entry, Item);
			}

			if(Ownership != null) {
			await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Ownership VALUES (
			null, @UID, @GID, @CharEntry, @Slot
			)", Ownership);
			CacheOwnership.TryAdd((Ownership.UID,Ownership.GID,Ownership.Slot), Ownership);
			}

			if(Account != null) {
			await _db.DbConn.ExecuteAsync(@"INSERT OR IGNORE INTO Account VALUES (
			null, @UID, @GID, @Slot
			)", Account);
			CacheAccount.TryAdd((Account.UID,Account.GID), Account);
			}
		}
		public static async Task UpdateAsync(Chr Chr = null, Account Account = null,Ownership Ownership = null, Inventory Inventory = null, Item Item = null)
		{
			if(Chr != null)
			{
			await _db.DbConn.ExecuteAsync(@"UPDATE Character SET
			Name = @Name, Gender = @Gender, Race = @Race, Age = @Age, Desc = @Desc, Image = @Image,
			HPC = @HPC, HPM = @HPM, ENC = @ENC, ENM = @ENM, Class = @Class, Sleight = @Sleight, Marksman = @Marksman, Dodge = @Dodge, Strenght = @Strenght,
			Thought = @Thought, Speed = @Speed, Intellegence = @Intellegence, Magic = @Magic, MagicEff = @MagicEff, Level = @Level, XP = @XP, SP = @SP, Balance = @Balance
			WHERE Entry = @Entry",Chr);
			ManageCharacter.UpdateCache(Chr);
			}

			if(Inventory != null)
			{
			await _db.DbConn.ExecuteAsync(@"UPDATE Inventory SET
			ItemAmount = @ItemAmount
			WHERE InventoryID = @InventoryID AND ItemID = @ItemID", Inventory);
			ManageCharacter.UpdateCache(Inventory: Inventory);
			}

			if(Item != null)
			{
			await _db.DbConn.ExecuteAsync(@"UPDATE Item
			Name = @Name, Desc = @Desc, Category = @Category, Attribute = @Attribute, Cost = @Cost
			WHERE Entry = @Entry", Item);
			ManageCharacter.UpdateCache(Item: Item);
			}

			if(Ownership != null)
			{
			await _db.DbConn.ExecuteAsync(@"UPDATE Ownership
			UID = @UID, GID = @GID, Slot = @Slot
			WHERE CharEntry = @CharEntry", Ownership);
			ManageCharacter.UpdateCache(Ownership: Ownership);
			}
			if(Account != null)
			{
			await _db.DbConn.ExecuteAsync(@"UPDATE Account SET
			Slot = @Slot
			WHERE UID = @UID AND GID = @GID",Account);
			ManageCharacter.UpdateCache(Account: Account);
			}
			
		}
		public static void UpdateCache(Chr Chr = null, Account Account = null, Ownership Ownership = null, Inventory Inventory = null, Item Item = null)
		{
			if(Chr != null) {
				Cache.TryRemove(Chr.Entry, out _);
				Cache.TryAdd(Chr.Entry, Chr);
			}

			if(Inventory != null) {
				CacheInventory.TryRemove((Inventory.InventoryID,Inventory.ItemID), out _);
				CacheInventory.TryAdd((Inventory.InventoryID,Inventory.ItemID), Inventory);
			}
			

			if(Item != null) {
				CacheItem.TryRemove(Item.Entry, out _);
				CacheItem.TryAdd(Item.Entry, Item);
			}
			

			if(Ownership != null) {
				CacheOwnership.TryRemove((Ownership.UID,Ownership.GID,Ownership.Slot), out _);
				CacheOwnership.TryAdd((Ownership.UID,Ownership.GID,Ownership.Slot), Ownership);
			}
			

			if(Account != null) {
				CacheAccount.TryRemove((Account.UID,Account.GID), out _);
				CacheAccount.TryAdd((Account.UID,Account.GID), Account);
			}
		}
		/*Here come the usefull functions.*/
		public static async Task<DiscordEmbed> EmbedProfileAsync(Chr Chr, string Owner = null)
		{
			var Embed = new DiscordEmbedBuilder {};
			string Alter = "[Original]";
			if(Chr.IsAlter == IsAlter.Yes)
			Alter = "[Alter]";
			Embed = Embed.WithAuthor(Alter + $" {Chr.Name}'s profile",null,Chr.Image);
			string Gender = "Female";
			if(Chr.Gender == Objects.Sql.Profile.Gender.Male)
				Gender = "Male";
			var race = Chr.Race;
			
			var Class = Chr.Class;
			
			Embed = Embed.WithDescription($"__**Age**__ `{Chr.Age}`\n__**Gender**__ `{Gender}`\n__**Race**__ `{race}`\n__**Class**__ `{Class}`\n\n__**Description**__ `{Chr.Desc}`");
			int neededXP = Convert.ToInt32(Chr.Level*100.57/4.2*Chr.Level);
			var field = new StringBuilder();
			field.Append($"**Health** `{Chr.HPC}|{Chr.HPM}`\n**Energy** `{Chr.ENC}|{Chr.ENM}`\n\n");
			field.Append($"**RB**\n[` Sleight {Chr.Sleight} `] - [` Marksman {Chr.Marksman} `] - [` Dodge {Chr.Dodge} `]\n\n");
			field.Append($"**Stats:**\n ***Strength({Chr.Strenght}) - Thought({Chr.Thought}) - Speed({Chr.Speed}) - Intellegence({Chr.Intellegence})***\n\n");
			field.Append($"**Magic({Chr.Magic}) | Magic-Eff({Chr.MagicEff})**\n\n");
			field.Append($"**Level {Chr.Level}**\n__Progress__[{Chr.XP}|{neededXP}]\nSP: `{Chr.SP}`\n\n" + Owner ?? "**Heaven X**");

			Embed = Embed.AddField("Stats/Skills", field.ToString());
			Embed = Embed.WithThumbnailUrl(Chr.Image);
			return Embed;
		}
		public static async Task<int> GetCountAsync(Tables table)
		{
			int count = 0;
			if(table == Tables.Character)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Character");
			if(table == Tables.Account)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Account");
			if(table == Tables.Ownership)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Ownership");
			if(table == Tables.Inventory)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Inventory");
			if(table == Tables.Item)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Item");
			return count;
		}
		public static async Task<int> GetCountAsync(Tables table, OwnerGet Owner = null, InvGet Inv = null)
		{
			int count = 0;
			if(table == Tables.Ownership)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Ownership WHERE UID = @UID AND GID = @GID",Owner);
			if(table == Tables.Inventory)
			count = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT (*) FROM Inventory WHERE CharEntry = @CharEntry", Inv);
			return count;
		}
		public static async Task<Chr> GetAllCache(ChrGet Chr)
		{
			if(Cache.TryGetValue(Chr.Entry, out var chr))
			{
				return chr;
			}
				return null;
		}
		public static async Task<Account> GetAllCache(AccountGet Acc)
		{
				if(CacheAccount.TryGetValue((Acc.UID, Acc.GID), out var acc))
				{
					return acc;
				}
				return null;
		}
		public static async Task<Ownership> GetAllCache(OwnerGet Owner)
		{
				if(CacheOwnership.TryGetValue((Owner.UID,Owner.GID,Owner.Slot), out var Own))
				{
					return Own;
				}
				return null;
		}
		public static async Task<Inventory> GetAllCache(InvGet Inv)
		{
				if(CacheInventory.TryGetValue((Inv.InventoryID,Inv.ItemID), out var INV))
				{
					return INV;
				}
				return null;
		}
		public static async Task<Item> GetAllCache(ItemGet Item)
		{
			
				if(CacheItem.TryGetValue(Item.ItemID, out var Items))
				{
					return Items;
				}
				return null;
		}
		public static async Task<Chr> GetChrByName(string name)
		{
			var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE name Like @name", new {name = "%" + name + "%"});
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Ownership> GetOwnerByEntry(int Entry)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Ownership>(@"SELECT * FROM Ownership WHERE CharEntry = @Entry", new { Entry });
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Chr> GetAll(ChrGet Chr)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Chr>(@"SELECT * FROM Character WHERE Entry = @Entry", Chr);
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Account> GetAll(AccountGet Acc)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Account>(@"SELECT * FROM Account WHERE UID = @UID AND GID = @GID", Acc);
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Ownership> GetAll(OwnerGet Owner)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Ownership>(@"SELECT * FROM Ownership WHERE UID = @UID AND GID = @GID AND Slot = @Slot", Owner);
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Inventory> GetAll(InvGet Inv)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Inventory>(@"SELECT * FROM Inventory WHERE InventoryID = @InventoryID AND ItemID = @ItemID", Inv);
				if(Chrs != null)
					return Chrs;
				return null;
		}
		public static async Task<Item> GetAll(ItemGet Item)
		{
				var Chrs = await _db.DbConn.QueryFirstOrDefaultAsync<Item>(@"SELECT * FROM Item WHERE ItemID = @ItemID", Item);
				if(Chrs != null)
					return Chrs;
				return null;
		}
	}
}
