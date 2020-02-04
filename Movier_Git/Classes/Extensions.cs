using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using MovierGit.Properties;

namespace MovierGit.Classes
{
	internal static class Extensions
	{
		public static void OpenFilm(string film, List<string> programs)
		{
			foreach (var program in programs)
				if (File.Exists(program))
				{
					Process.Start(program, film);
					return;
				}
		}

		public static string GetPage(string www)
		{
			string page;
			using (var client = new WebClient())
			{
				try
				{
					var pageData = client.DownloadData(www);
					page = Encoding.Default.GetString(pageData);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Problem z pobraniem strony: {www}\n{ex}");
					return null;
				}
			}
			return page;
		}

		public static string GetAddressLastSequence(string address)
		{
			var prefixLength = Global.LongPrefix.Length;		// 25
			var indexOfNoLetter = IndexOfNoLetter(address, prefixLength);
			var lastSequence = indexOfNoLetter < 1
				? address.Substring(prefixLength, address.Length - prefixLength)
				: address.Substring(prefixLength, indexOfNoLetter - prefixLength);
			return lastSequence;
		}

		public static int IndexOfNoLetter(string word, int startIndex)
		{
			for (var i = startIndex; i < word.Length; i++)
				if (!char.IsLetterOrDigit(word[i]))
					return i;
			return word.Length;
		}

		public static string SetVersionsNames(string version)
		{
			if (!version.Contains("wersja"))
				return Resources.OneVersionMovie;

			var versionName = "";
			var lastIndex = version.LastIndexOf('=') + 1;
			versionName = version.Substring(lastIndex, version.Length - lastIndex);

			return versionName;
		}

		public static IDictionary<char, char> GetDictionaryWithCode()
		{
			string alphabet = "", alphabetEncoded = "";
			var vocabularyCode = new Dictionary<char, char>();

			for (var i = 65; i <= 122; i++)
			{
				var shiftInAscii = 13;
				if (i >= 91 && i < 97)
					continue;
				if ((i >= 78 && i < 91) || i >= 110)
					shiftInAscii = -13;

				var j = i + shiftInAscii;
				var c = (char) i;
				alphabet += c;
				var c2 = (char) j;
				alphabetEncoded += c2;
			}
			for (var i = 0; i < alphabetEncoded.Length; i++)
				vocabularyCode[alphabet[i]] = alphabetEncoded[i];

			return vocabularyCode;
		}

		public static string RemoveSubstring(string word, int start_index, int length)
		{
			var firstPart = word.Substring(0, start_index);
			var secondPart = word.Substring(start_index + length);
			return firstPart + secondPart;
		}

		public static string DecodeOneVersion(string text)
		{
			IDictionary<char, char> alphabet = GetDictionaryWithCode();
			var result = "";

			foreach (var letter in text)
				result += alphabet.ContainsKey(letter) ? alphabet[letter] : letter;

			result = RemoveSubstring(result, result.Length - 7, 3);
			return result;
		}

		public static ApplicationSettings Deserialize(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			ApplicationSettings applicationSettings;
			var xmlSerializer = new XmlSerializer(typeof(ApplicationSettings));
			using (Stream stream = File.OpenRead(fileName))
				applicationSettings = (ApplicationSettings) xmlSerializer.Deserialize(stream);

			return applicationSettings;
		}

		//private static void Serialize<T>(T obiekt, string fileName)
		//{
		//	var xmlSerializer = new XmlSerializer(typeof(T));
		//	using (Stream stream = File.Create(fileName))
		//		xmlSerializer.Serialize(stream, obiekt);
		//}

		//public static T Deserialize<T>(string fileName) where T : new()
		//{
		//	T obiekt = new T();
		//	var xmlSerializer = new XmlSerializer(typeof(T));
		//	using (Stream stream = File.OpenRead(fileName))
		//		obiekt = (T) xmlSerializer.Deserialize(stream);
		//	return obiekt;
		//}
	}
}