using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class FamilyManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<ulong, Family> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<ulong, Family>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Marriage (
			PID BIGINT PRIMARY KEY,
            PartnerId BIGINT
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(Family obj) {
			if (!Cache.ContainsKey(obj.PID))
            {
                Cache.TryAdd(obj.PID, obj);
            }
			else
            {
                var result = await _db.DbConn.QueryFirstOrDefaultAsync<Family>("SELECT * FROM Marriage WHERE PID = @PID",
																	 new { obj.PID });
                Cache.TryUpdate(obj.PID, obj, result);
            }	
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Marriage VALUES (
			@PID, @PartnerId
			) ON CONFLICT (PID) DO UPDATE SET
			PartnerId = @PartnerId",obj);
		}

		public static async Task<Family> GetAsync(ulong PID) {
			if (Cache.TryGetValue(PID, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Family>("SELECT * FROM Marriage WHERE PID = @PID",
																	 new { PID });
			if (result != null)
				Cache.TryAdd(result.PID, result);
			return result;
		}
        public static async Task RemoveAsync(ulong PID)
        {
            if (Cache.TryGetValue(PID, out var result))
                {
                    Cache.TryRemove(result.PID, out _);
                }
				
			await _db.DbConn.ExecuteAsync("DELETE FROM Marriage WHERE PID = @PID",
																	 new { PID });
        }
        
	}
}
