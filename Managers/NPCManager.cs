using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class NPCManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<int, NPC> Cache { get; private set; }
        public static ConcurrentDictionary<ulong, Proxy> ProxyCache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<int, NPC>();
            ProxyCache = new ConcurrentDictionary<ulong, Proxy>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS NPC (
			NPCID INT PRIMARY KEY,
            Name TEXT, Desc TEXT, Ref TEXT, HP_current INT, HP_max INT,
            RBonus INT, MBonus INT, MagBonus INT, ATK INT
			)");
		}

		/* We won't manually waste teim writing queries, just create a new
		 * object, fill the values and call this method to create or automatically
		 * update it. */
		public static async Task SyncAsync(NPC obj) {
			if (!Cache.ContainsKey(obj.NPCID))
				Cache.TryAdd(obj.NPCID, obj);
			await _db.DbConn.ExecuteAsync(@"INSERT INTO NPC VALUES (
			@NPCID, @Name, @Desc, @Ref, @HP_current, @HP_max, @RBonus, @MBonus,@MagBonus, @ATK
			) ON CONFLICT (NPCID) DO UPDATE SET
			Name = @Name, Desc = @Desc, Ref = @Ref, HP_current = @HP_current,
            HP_max = @HP_max, ATK = @ATK, RBonus = @RBonus, MBonus = @MBonus, MagBonus = @MagBonus",obj);
            await NPCManager.CacheAsync(obj.NPCID);
		}

		public static async Task<NPC> GetAsync(int ID) {
			if (Cache.TryGetValue(ID, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE NPCID = @ID",
																	 new { ID });
			if (result != null)
				Cache.TryAdd(result.NPCID, result);
			return result;
		}
        public static async Task<int> CountAsync() {
            var c = await _db.DbConn.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM NPC");
            return c;
        }
        public static async Task<NPC> SearchAsync(string name) {
        var chr = Cache.FirstOrDefault(kv => kv.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value; /* Equals is good because it gives more speed as far as im aware */
        if (chr != null)
            return chr;

        chr = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE Name like @name", new { name = "%" + name + "%" });
        if (chr != null)
            Cache.TryAdd(chr.NPCID, chr);
        return chr;
        }
        public static async Task ProxyAsync(string name, ulong Author)
        {
            var chr = Cache.FirstOrDefault(kv => kv.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value; /* Equals is good because it gives more speed as far as im aware */
            if (chr != null)
            {
                var prox = new Proxy();
                prox.NPCID = chr.NPCID;
                prox.PID = Author;
                ProxyCache.TryAdd(Author, prox);
            }

            chr = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE Name like @name", new { name = "%" + name + "%" });
            if (chr != null)
            {
                var prox = new Proxy();
                prox.NPCID = chr.NPCID;
                prox.PID = Author;
                ProxyCache.TryAdd(Author, prox);
            }
                
            
        }
         public static void ProxySelfAsync(ulong Author)
        {
            ProxyCache.TryRemove(Author, out _);
        }
        public static async Task<Proxy> ProxyGetAsync(ulong Author)
        {   
            var res = new Proxy();
            if(ProxyCache.TryGetValue(Author, out res))
            return res;
            return null;
        }
        public static async Task HealAsync(int ID)
        {				
			var result = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE NPCID = @ID",
																	 new { ID });
            result.HP_current = result.HP_max;
			await NPCManager.SyncAsync(result);
			
        }
        public static async Task CacheAsync(int PID)
		{
			if (!Cache.ContainsKey(PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE NPCID = @PID",
																		new { PID });
				if (result != null)
				{
					Cache.TryAdd(result.NPCID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<NPC>("SELECT * FROM NPC WHERE NPCID = @PID",
																		new { PID });
				if (result2 != null)
				{
					var cc = new NPC();
					Cache.TryGetValue(PID, out cc);
					Cache.TryUpdate(PID, result2, cc);
				}
			}
		}
	}
}
