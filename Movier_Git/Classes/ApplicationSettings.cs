using System.Collections.Generic;

namespace MovierGit.Classes
{
	public class ApplicationSettings
	{
		public List<string> Programs { get; set; } = new List<string>();

		public ApplicationSettings() { }	// musi być
		public ApplicationSettings(string program)
		{
			Programs.Add(program);
		}
	}
}
