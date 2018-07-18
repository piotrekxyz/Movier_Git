using System.Collections.Generic;

namespace Movier_Git.Classes
{
	public class ApplicationSettings
	{
		public List<string> Programs { get; set; } = new List<string>();

		public ApplicationSettings() { }

		public ApplicationSettings(string program) { Programs.Add(program); }
	}
}
