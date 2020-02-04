using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using Movier_Git.Properties;

namespace Movier_Git.Classes
{
	static class Extensions
	{
		static public void OpenFilm(string film, List<string> programs)
		{
			foreach (var program in programs)
			{
				if (File.Exists(program))
				{
					Process.Start(program, film);
					return;
				}
			}
		}

		static public string GetPage(string www)
		{
			string page = null;
			byte[] page_data = null;
			using (WebClient Client = new WebClient())
			{
				try
				{
					page_data = Client.DownloadData(www);
					page = Encoding.Default.GetString(page_data);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Problem z pobraniem strony: {www}\n{ex}");
					return null;
				}
			}
			return page;
		}

		static public string GetAddressLastSequence(string address)
		{
			int prefixLength = Global.longPrefix.Length;   // 25
			int index_of_noLetter = IndexOfNoLetter(address, prefixLength);
			string lastSequence = index_of_noLetter < 1 ? address.Substring(prefixLength, address.Length - prefixLength) : address.Substring(prefixLength, index_of_noLetter - prefixLength);
			return lastSequence;
		}

		static public int IndexOfNoLetter(string word, int startIndex)
		{
			for (int i = startIndex; i < word.Length; i++)
				if (!char.IsLetterOrDigit(word[i]))
					return i;
			return word.Length;
		}

		static public string SetVersionsNames(string version)
		{
			if (!version.Contains("wersja"))
				return Resources.OneVersionMovie;

			string versionName = "";
			int last_index = version.LastIndexOf('=') + 1;
			versionName = version.Substring(last_index, version.Length - last_index);

			return versionName;
		}

		static public IDictionary<char, char> GetDictionaryWithCode()
		{
			string alphabet = "", alphabet_encoded = "";
			IDictionary<char, char> vocabularyCode = new Dictionary<char, char>();

			for (int i = 65; i <= 122; i++)
			{
				int shiftInASCII = 13;
				if (i >= 91 && i < 97)
					continue;
				if ((i >= 78 && i < 91) || i >= 110)
					shiftInASCII = -13;

				int j = i + shiftInASCII;
				char c = (char)i;
				alphabet += c;
				char c2 = (char)j;
				alphabet_encoded += c2;
			}
			for (int i = 0; i < alphabet_encoded.Length; i++)
				vocabularyCode[alphabet[i]] = alphabet_encoded[i];

			return vocabularyCode;
		}

		static public string RemoveSubstring(string word, int start_index, int length)
		{
			string first_part = word.Substring(0, start_index);
			string second_part = word.Substring(start_index + length);
			return first_part + second_part;
		}

		static public string DecodeOneVersion(string text)
		{
			IDictionary<char, char> alphabet = GetDictionaryWithCode();
			string result = "";

			foreach (char letter in text)
				result += alphabet.ContainsKey(letter) ? alphabet[letter] : letter;

			result = RemoveSubstring(result, result.Length - 7, 3);
			return result;
		}

		public static ApplicationSettings Deserialize(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			ApplicationSettings applicationSettings = new ApplicationSettings();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ApplicationSettings));
			using (Stream stream = File.OpenRead(fileName))
				applicationSettings = (ApplicationSettings)xmlSerializer.Deserialize(stream);

			return applicationSettings;
		}

		//private static void Serialize<T>(T obiekt, string fileName)
		//{
		//	XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		//	using (Stream stream = File.Create(fileName))
		//		xmlSerializer.Serialize(stream, obiekt);
		//}

		//public static T Deserialize<T>(string fileName) where T : new()
		//{
		//	T obiekt = new T();
		//	XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		//	using (Stream stream = File.OpenRead(fileName))
		//		obiekt = (T)xmlSerializer.Deserialize(stream);
		//	return obiekt;
		//}
	}
}
