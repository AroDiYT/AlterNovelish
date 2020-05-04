using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class CurrencyManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<ulong, Currency> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<ulong, Currency>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Currency (
			PID BIGINT PRIMARY KEY,
            Balance INT
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(Currency obj) {
			if (!Cache.ContainsKey(obj.PID))
				Cache.TryAdd(obj.PID, obj);
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Currency VALUES (
			@PID, @Balance
			) ON CONFLICT (PID) DO UPDATE SET
			Balance = @Balance",obj);
            await CurrencyManager.CacheAsync(obj.PID);
		}

		public static async Task<Currency> GetAsync(ulong name) {
			if (Cache.TryGetValue(name, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Currency>("SELECT * FROM Currency WHERE PID = @name",
																	 new { name });
			if (result != null)
				Cache.TryAdd(result.PID, result);
			return result;
		}
        public static async Task AddAsync(ulong ID, int Amount)
        {
            var result = await _db.DbConn.QueryFirstOrDefaultAsync<Currency>("SELECT * FROM Currency WHERE PID = @ID",
																	 new { ID });
            await _db.DbConn.ExecuteAsync("UPDATE Currency SET BALANCE = @b WHERE PID = @ID", new { b = result.Balance + Amount, ID });
            await CurrencyManager.CacheAsync(ID);
        }
        public static async Task TakeAsync(ulong ID, int Amount)
        {
            var result = await _db.DbConn.QueryFirstOrDefaultAsync<Currency>("SELECT * FROM Currency WHERE PID = @ID",
																	 new { ID });
            await _db.DbConn.ExecuteAsync("UPDATE Currency SET BALANCE = @b WHERE PID = @ID", new { b = result.Balance - Amount, ID });
            await CurrencyManager.CacheAsync(ID);
        }
        public static async Task CacheAsync(ulong PID)
		{
			if (!Cache.ContainsKey(PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<Currency>("SELECT * FROM Currency WHERE PID = @PID",
																		new { PID });
				if (result != null)
				{
					Cache.TryAdd(result.PID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<Currency>("SELECT * FROM Currency WHERE PID = @PID",
																		new { PID });
				if (result2 != null)
				{
					var cc = new Currency();
					Cache.TryGetValue(PID, out cc);
					Cache.TryUpdate(PID, result2, cc);
				}
			}
		}
	}
}
