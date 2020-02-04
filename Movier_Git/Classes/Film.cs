using System.Collections.Generic;

namespace MovierGit.Classes
{
	internal class Film
	{
		public string AddressPortal { get; set; }
		public IList<Version> Versions = new List<Version>();

		//public Film(string addressPortal) => this.addressPortal = addressPortal;
		public Film(string addressPortal)
		{
			AddressPortal = addressPortal;
		}
	}
}