using System.Collections.Generic;

namespace Movier_Git.Classes
{
	class Film
	{
		public string addressPortal;
		public IList<Version> versions = new List<Version>();

		//public Film(string addressPortal) => this.addressPortal = addressPortal;
		public Film(string addressPortal)
		{
			this.addressPortal = addressPortal;
		}
	}
}