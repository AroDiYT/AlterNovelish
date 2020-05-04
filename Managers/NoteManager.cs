using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class NoteManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<string, Note> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<string, Note>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Notes (
			Name TEXT PRIMARY KEY,
			Author UNSIGNED BIGINT,
			Value TEXT
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(Note obj) {
			if (!Cache.ContainsKey(obj.Name))
				Cache.TryAdd(obj.Name, obj);
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Notes VALUES (
			@Name, @Author, @Value
			) ON CONFLICT (Name) DO UPDATE SET
			Value = @Value",obj);
		}

		public static async Task<Note> GetAsync(string name) {
			if (Cache.TryGetValue(name, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Note>("SELECT * FROM Notes WHERE Name = @name COLLATE NOCASE",
																	 new { name });
			if (result != null)
				Cache.TryAdd(result.Name, result);
			return result;
		}
	}
}
