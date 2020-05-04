namespace BotTemplate.Objects.Sql {
	/* A note object, it's simple and has no
	 * logic; it only stores data. */
	public class Note {
		public string Name { get; set; }
		public ulong Author { get; set; }
		public string Value { get; set; }
	}
	public class Character {
		public ulong PID { get; set;}
		public string Name { get; set; }
		public string Age { get; set; }
		public string Gender { get; set; }
		public string Desc { get; set; }
		public string Ref { get; set; }
		public int Level { get; set; }
		public int Xp { get; set; }
	}
	public class Currency {
		public ulong PID { get; set; }
		public int Balance { get; set; }
	}
	public class Stats {
		public ulong PID { get; set; }
		public int HP_max { get; set; }
		public int HP_current { get; set; }
		public int ATK { get; set; }
		public int MELEE { get; set; }
		public int RANGED { get; set; }
		public int MAGIC { get; set; }
		public int Energy_max { get; set; }
		public int Energy_current { get; set; }
		public int SP { get; set; }
	}
	public class Spells {
		public int ID { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ReqMagic { get; set; }
		public int Value { get; set; }
		public string Category { get; set; }
		public int Energy_Cost { get; set; }
	}
	public class Inventory {
		public int ID { get; set; }
		public int Amount { get; set; }
		
	}
	public class Items {
		public int ID { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public string Category { get; set; }
		public int Cost { get; set; }
		public int Value { get; set; }
	}
	public class Family {
		public ulong PID { get; set; }
		public ulong PartnerId { get; set; }

	}
	public class Channel {
		public ulong ID { get; set; }
		public bool RP { get; set; }
	}
}
