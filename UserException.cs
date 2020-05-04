using System;

namespace BotTemplate {
	/// <summary>
	///   Represents an error that is made because of a user running a command.
	///   Use this to quickly bail out of a command and leave a meaningful reason why.
	/// </summary>
	[Serializable]
	public class UserException : Exception {
		public UserException() {
		}

		public UserException(string message)
			: base(message) {
		}
	}
}
