using System.Data.SQLite;

namespace BotTemplate {
	public class Database {
		public SQLiteConnection DbConn { get; private set; }

		public Database(string file) {
			this.DbConn = new SQLiteConnection($"Data Source={file}");
			this.DbConn.Open();
		}
	}
}
