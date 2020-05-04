namespace BotTemplate.Objects.Json {
	/* Object for Config file */
	/* config should look like:
	 * {
	 * 	"token": "something",
	 * 	"prefixes": ["one", "two"]
	 * }
	 * */
	public class BotConfig {
		public string Token { get; set; }
		public string[] Prefixes { get; set; }
	}
}
