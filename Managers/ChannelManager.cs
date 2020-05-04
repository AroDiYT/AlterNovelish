using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class ChannelManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<ulong, Channel> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<ulong, Channel>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Channel (
			ID BIGINT PRIMARY KEY,
            RP BOOL
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(Channel obj) {
			if (!Cache.ContainsKey(obj.ID))
            {
                Cache.TryAdd(obj.ID, obj);
            }
			else
            {
                var result = await _db.DbConn.QueryFirstOrDefaultAsync<Channel>("SELECT * FROM Channel WHERE ID = @ID",
																	 new { obj.ID });
                Cache.TryUpdate(obj.ID, obj, result);
            }	
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Channel VALUES (
			@ID, @RP
			) ON CONFLICT (ID) DO UPDATE SET
			RP = @RP",obj);
		}

		public static async Task<Channel> GetAsync(ulong ID) {
			if (Cache.TryGetValue(ID, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Channel>("SELECT * FROM Channel WHERE ID = @ID",
																	 new { ID });
			if (result != null)
				Cache.TryAdd(result.ID, result);
			return result;
		}
        public static async Task RemoveAsync(ulong ID)
        {
            if (Cache.TryGetValue(ID, out var result))
                {
                    Cache.TryRemove(result.ID, out _);
                }
				
			await _db.DbConn.ExecuteAsync("DELETE FROM Channel WHERE ID = @ID",
																	 new { ID });
        }
        
	}
}
