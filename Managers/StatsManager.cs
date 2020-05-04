using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Dapper;

using BotTemplate.Objects.Sql;

namespace BotTemplate.Managers {
	public static class StatsManager {
		private static Database _db;

		/* Caching is very important because each time you query the db, it
		 * creates a completley new object that wastes memory and causes many
		 * syncing issues. So, ALWAYS TRY TO USE THE CACHE */
		public static ConcurrentDictionary<ulong, Stats> Cache { get; private set; }

		/* Put initialization logic here */
		public static void Initialize(Database db) {
			_db = db;

			Cache = new ConcurrentDictionary<ulong, Stats>();

			_db.DbConn.Execute(@"CREATE TABLE IF NOT EXISTS Stats (
			PID BIGINT PRIMARY KEY,
            HP_max INT,
            HP_current INT,
            ATK INT,
            MELEE INT,
            RANGED INT,
            MAGIC INT,
            Energy_max INT,
            Energy_current INT,
            SP INT

			)");
		}
		public static async Task SyncAsync(Stats obj) {
			await _db.DbConn.ExecuteAsync(@"INSERT INTO Stats VALUES (
			@PID, @HP_max, @HP_current, @ATK, @MELEE, @RANGED, @MAGIC, @Energy_max, @Energy_current, '0'
            ) ON CONFLICT (PID) DO UPDATE SET
			PID = @PID, HP_max = @HP_max, HP_current = @HP_current, ATK = @ATK, MELEE = @MELEE,
            RANGED = @RANGED, MAGIC = @MAGIC, Energy_max = @Energy_max, Energy_current = @Energy_current, SP = @SP",obj);
			if (!Cache.ContainsKey(obj.PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																		new { obj.PID });
				if (result != null)
				{
					Cache.TryAdd(result.PID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																		new { obj.PID });
				if (result2 != null)
				{
					Cache.TryUpdate(obj.PID, result2, null);
				}
			}
            
		}

		public static async Task<Stats> GetAsync(ulong PID) {
			if (Cache.TryGetValue(PID, out var result))
				return result;
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																	 new { PID });
			if (result != null)
				Cache.TryAdd(result.PID, result);
			return result;
		}
        public static async Task UpdateAsync(ulong PID, string Stat, int Amount) {
            if (Cache.TryGetValue(PID, out var result))
            {
                var res = result;
                if(Stat == "HP")
                    result.HP_max += Amount;
                if(Stat == "ATK")
                    result.ATK += Amount;
                if(Stat == "MELEE")
                    result.MELEE += Amount;
                if(Stat == "RANGED")
                    result.RANGED += Amount;
                if(Stat == "MAGIC")
                    result.MAGIC += Amount;
                if(Stat == "ENERGY")
                    result.Energy_max += Amount;
                Cache.TryUpdate(PID, result, res);
            }
			
			result = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																	 new { PID });
            if(result == null)
                return;
            if(Stat == "HP")
                    result.HP_max += Amount;
            if(Stat == "ATK")
                    result.ATK += Amount;
            if(Stat == "MELEE")
                    result.MELEE += Amount;
            if(Stat == "RANGED")
                    result.RANGED += Amount;
            if(Stat == "MAGIC")
                    result.MAGIC += Amount;
            if(Stat == "ENERGY")
                    result.Energy_max += Amount;
            await _db.DbConn.ExecuteAsync(@"UPDATE Stats SET
			HP_max = @HP_max, HP_current = @HP_current, ATK = @ATK, MELEE = @MELEE,
            RANGED = @RANGED, MAGIC = @MAGIC, Energy_max = @Energy_max, Energy_current = @Energy_current, SP = @SP WHERE PID = @PID",result);
            
        }
        public static async Task HealAsync(ulong ID)
        { 
			var cc = await StatsManager.GetAsync(ID);
            var occ = cc;
			cc.HP_current = cc.HP_max;
            cc.Energy_current = cc.Energy_max;
            Cache.TryUpdate(ID, cc, occ);
			await _db.DbConn.ExecuteAsync(@"UPDATE Stats SET
			HP_max = @HP_max, HP_current = @HP_current, ATK = @ATK, MELEE = @MELEE,
            RANGED = @RANGED, MAGIC = @MAGIC, Energy_max = @Energy_max, Energy_current = @Energy_current, SP = @SP WHERE PID = @PID",cc);
        }
		public static async Task RestAsync(ulong ID)
        { 
			var cc = await StatsManager.GetAsync(ID);
            var occ = cc;
			cc.HP_current += cc.HP_max/2;
            cc.Energy_current += cc.Energy_max/2;
			if(cc.HP_current > cc.HP_max)
				cc.HP_current = cc.HP_max;
			if(cc.Energy_current > cc.Energy_max)
				cc.Energy_current = cc.Energy_max;
            Cache.TryUpdate(ID, cc, occ);
			await _db.DbConn.ExecuteAsync(@"UPDATE Stats SET
			HP_max = @HP_max, HP_current = @HP_current, ATK = @ATK, MELEE = @MELEE,
            RANGED = @RANGED, MAGIC = @MAGIC, Energy_max = @Energy_max, Energy_current = @Energy_current, SP = @SP WHERE PID = @PID",cc);
        }
        public static async Task SPAsync(ulong ID, int Amount)
        {
            var cc = await StatsManager.GetAsync(ID);
            var occ = cc;
            cc.SP += Amount;
            Cache.TryUpdate(ID, cc, occ);
            await _db.DbConn.ExecuteAsync(@"UPDATE Stats SET
			HP_max = @HP_max, HP_current = @HP_current, ATK = @ATK, MELEE = @MELEE,
            RANGED = @RANGED, MAGIC = @MAGIC, Energy_max = @Energy_max, Energy_current = @Energy_current, SP = @SP WHERE PID = @PID",cc);
        }
        public static async Task CacheAsync(ulong PID)
		{
			if (!Cache.ContainsKey(PID))
            {
				var result = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																		new { PID });
				if (result != null)
				{
					Cache.TryAdd(result.PID, result);
				}
			}
			else
			{
				var result2 = await _db.DbConn.QueryFirstOrDefaultAsync<Stats>("SELECT * FROM Stats WHERE PID = @PID",
																		new { PID });
				if (result2 != null)
				{
					Cache.TryUpdate(PID, result2, null);
				}
			}
		}
	}
}
