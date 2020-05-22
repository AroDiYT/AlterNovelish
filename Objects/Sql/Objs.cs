namespace BotTemplate.Objects.Sql.Profile {
	public class Modifiers {
		public int Attack { get; set; }
		public int Defend { get; set; }
		public int DamageBonus { get; set; }
	}
	public enum ChannelCategory {
		Rp,
		Chat,
		Bot
	}
	public class Channel {
		public ulong ID { get; set; }
		public ChannelCategory Category { get; set; }
		public int XP { get; set; }
	}
	public enum Tables {
		Character = 0,
		Account = 1,
		Ownership = 2,
		Inventory = 3,
		Item = 4
	}
	
		public class ChrGet {
			public int Entry { get; set; }
		}
		

		public class OwnerGet {
			public ulong UID { get; set; }
			public ulong GID { get; set; }
			public int Slot { get; set; }
		}
		
		public class AccountGet {
			public ulong UID { get; set; }
			public ulong GID { get; set; }
		}
		public class InvGet {
			public int InventoryID { get; set; }
			public int ItemID { get; set; }
		}
		public class ItemGet {
			public int ItemID { get; set; }
		}

	 public class Chr {
		 public int Entry { get; set; }
		 public string Name { get; set; }
		 public Gender Gender { get; set; }
		 public Race Race { get; set; }
		 public int Age { get; set; }
		 public string Desc { get; set; }
		 public string Image { get; set; }

		 public int HPC { get; set; }
		 public int HPM { get; set; }
		 
		 public int ENC { get; set; }
		 public int ENM { get; set; }

		 public Classes Class { get; set; }

		 public int Sleight { get; set; }
		 public int Marksman { get; set; }
		 public int Dodge { get; set; }

		 public int Strenght { get; set; }
		 public int Thought { get; set; }
		 public int Speed { get; set; }
		 public int Intellegence { get; set; }

		 public int Magic { get; set; }
		 public int MagicEff { get; set; }

		 public int Level { get; set; }
		 public int XP { get; set; }

		 public IsAlter IsAlter { get; set; }

		 public int Balance { get; set; }

		 public int InventoryID { get; set; }
		 public int SP { get; set; }
	 }
	 public class Ownership {
		 public ulong UID { get; set; }
		 public ulong GID { get; set; }
		 public int CharEntry { get; set; }
		 public int Slot { get; set; }
	 }
	 public class Inventory {
		 public int InventoryID { get; set; }
		 public int ItemID { get; set; }
		 public int ItemAmount { get; set; }
	 }
	 public class Item {
		 public int Entry { get; set; }
		 public string Name { get; set; }
		 public string Desc { get; set; }
		 public ItemCategory Category { get; set; }
		 public int Attribute { get; set; }
		 public int Cost { get; set; }
	 }
	 public class Account {
		 public ulong UID { get; set; }
		 public ulong GID { get; set; }
		 public int Slot { get; set; }
	 }
	 public enum Gender {
		 Female = 1,
		 Male = 0
	 }
	 public enum ItemCategory {
		 Weapon = 0,
		 Food = 1,
		 Armor = 2,
		 Collectables = 3
	 }
	 public enum Race {
		 Human = 0, //Town
		 Elf = 1, //Forest
		 Fairy = 2, //Forest
		 Neko = 3, //Forest or Town
		 Celeste = 4, //Void
		 Cyborg = 5, //Town
		 Spirit = 6, //Astral
		 Ghost = 7, //Astral
		 Kitsune = 8, //Forest
		 Dragonoid //Cave
	 }
	 public enum Classes {
		 Warrior = 0,
		 Archer = 1,
		 Mage = 2,
		 Thief,
		 Defender
	 }
	 public enum IsAlter {
		 Yes,No
	 }
	 /*
	 public class RaceURL {
		 public GenderRace Human { get; set; }
		 public GenderRace Elf { get; set; }
		 public GenderRace Fairy { get; set; }
		 public GenderRace Neko { get; set; }
		 public GenderRace Celeste { get; set; }
		 public GenderRace Cyborg { get; set; }
		 public GenderRace Spirit { get; set; }
		 public GenderRace Ghost { get; set; }
	 }
	 
	 public class GenderRace {
		 public class Human {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709723363869982740/c933117c7bff5ab14a1815458f472f03.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709722878353866792/87462-anime-fantasy_art-748x532.png";
			 public string NonBinary = "";
		 }
		 public class Elf {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709724198146146334/55b2a219177a525a3bf87378356d98cf.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709724327712653313/e93e506bcd45f861bafa403f7c818dd9.png";
		 }
		 public class Fairy {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709724929947467827/bfcbd2e2451a6654a0a60f4fc0ff4197.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709724570352877618/6beb667494a3274c659e137e30da9715.png";
		 }
		 public class Neko {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709725105286283354/da62d76cee16ab3c7e805e50b0ae350b.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709725463261478922/Miqo27te.png";
		 }
		 public class Celeste {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709726279913701386/40fe02c97820e32bac7a83da3d217117--kagerou-project.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709725676021874758/girl-stone-gun-pose-wallpaper-preview.png";
		 }
		 public class Cyborg {
			 public string Male = "https://cdn.discordapp.com/attachments/708249001903783986/709726474936254594/b1e9cb30f13b862e2d28f37657f23e94.png";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709726663600242749/4d281b5b4395cf0591d0f28a8a061e32.png";
		 }
		 public class Spirit {
			 public string Male = "";
			 public string Female = "https://cdn.discordapp.com/attachments/708249001903783986/709726813714382888/9821a3e55eba0d034d361fb77f6af9b1.png";
		 }
		 public class Ghost {
			 public string Male = "";
			 public string Female = "";
		 }*/
		 
}

namespace BotTemplate.Objects.Sql.Note {
	public class Note {
		public ulong UID { get; set; }
		public ulong GID { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
	}
}