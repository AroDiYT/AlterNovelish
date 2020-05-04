using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class CharManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<ulong, Character> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<ulong, Character>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Characters (
			PID BIGINT PRIMARY KEY,
            Name TEXT,
			Age TEXT,
			Gender Text,
			Desc TEXT,
            Ref TEXT,
			Level INTEGER,
			Xp INTEGER
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(Character obj) {
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Characters VALUES (
			@PID, @Name, @Age, @Gender, @Desc, @Ref, '1', '0'
			) ON CONFLICT (PID) DO UPDATE SET
			Name = @Name, Age = @Age, Gender = @Gender,  Desc = @Desc, Ref = @Ref",obj);
			if (!Cache.ContainsKey(obj.PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",obj);
				if (result != null)
				{
					Cache.TryAdd(result.PID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",obj);
				if (result2 != null)
				{
					var cc = new Character();
					Cache.TryGetValue(obj.PID, out cc);
					Cache.TryUpdate(obj.PID, result2, cc);
				}
			}
            
		}

		public static async Task<Character> GetAsync(ulong PID) {
			if (Cache.TryGetValue(PID, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",
																	 new { PID });
			if (result != null)
				Cache.TryAdd(result.PID, result);
			return result;
		}
        public static async Task<Character> SearchAsync(string name) {
        var chr = Cache.FirstOrDefault(kv => kv.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value; /* Equals is good because it gives more speed as far as im aware */
        if (chr != null)
            return chr;

        chr = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE Name like @name", new { name = "%" + name + "%" });
        if (chr != null)
            Cache.TryAdd(chr.PID, chr);
        return chr;
        }
		public static async Task LevelAsync(ulong PID)
		{
			if (Cache.TryGetValue(PID, out var result))
            {
                var res = result;

				result.Xp -= result.Level*100/2*4;
				if(result.Xp < 0)
					result.Xp = 0;
				result.Level += 1;
				
                Cache.TryUpdate(PID, result, res);
            }
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",
																	 new { PID });
			result.Xp -= result.Level*100/2*4;
			if(result.Xp < 0)
					result.Xp = 0;
			result.Level += 1;
			await _db.DbConn.ExecuteAsync("UPDATE Characters SET Level = @Level, Xp = @Xp WHERE PID = @PID",result);
			await StatsManager.SPAsync(PID, 3);
		}
		public static async Task XpAsync(ulong PID, int Amount)
		{
			if (Cache.TryGetValue(PID, out var result))
            {
                var res = result;
				result.Xp += Amount;
                Cache.TryUpdate(PID, result, res);
            }
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",
																	 new { PID });
			result.Xp += Amount;
			if(result.Xp >= (result.Level*100/2*4))
			{
				await CharManager.LevelAsync(result.PID);
			}
			else
			{
				await _db.DbConn.ExecuteAsync("UPDATE Characters SET Xp = @Xp WHERE PID = @PID",
																							result);
			}
			
		}
		public static async Task CacheAsync(ulong PID)
		{
			if (!Cache.ContainsKey(PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",
																		new { PID });
				if (result != null)
				{
					Cache.TryAdd(result.PID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<Character>("SELECT * FROM Characters WHERE PID = @PID",
																		new { PID });
				if (result2 != null)
				{
					var cc = new Character();
					Cache.TryGetValue(PID, out cc);
					Cache.TryUpdate(PID, result2, cc);
				}
			}
		}
	}
}
