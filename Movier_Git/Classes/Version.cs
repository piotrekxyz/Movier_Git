namespace Movier_Git.Classes
{
	class Version
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string CryptoVersion { get; set; }
		public string DecodeVersion { get; set; }

		public Version(string name, string address, string crypto, string decode)
		{
			Name = name;
			Address = address;
			CryptoVersion = crypto;
			DecodeVersion = decode;
		}
	}
}
