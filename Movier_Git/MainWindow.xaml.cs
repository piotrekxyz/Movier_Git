using Movier_Git.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Movier_Git
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			InitialApplicationSettings();
		}
		ApplicationSettings applicationSettings;
		List<RadioButton> radioButtonsList = new List<RadioButton>();
		Film movie;

		public void InitialApplicationSettings()
		{
			applicationSettings = Extensions.Deserialize(Global.settings);
			applicationSettings = applicationSettings is null ? new ApplicationSettings(Global.chrome86) : applicationSettings;
		}

		async void Go_bt_Click(object sender, RoutedEventArgs e)
		{
			string portal = Address_portal_tb.Text;
			if (string.IsNullOrEmpty(portal) || portal.Length < 27)
				return;
			CleanGUI();
			Go_bt.IsEnabled = false;
			movie = new Film(portal);
			await GetAllVersions();
			MakeRadioButtons();
			ShowAllVersions();
			Go_bt.IsEnabled = true;
		}

		void ShowAllVersions()
		{
			if (radioButtonsList.Count < 1)
			{
				Info_lb.Content = "NIE UDAŁO SIĘ POBRAĆ !!";
				return;
			}
			radioButtonsList.ForEach(rb => StackPanel1.Children.Add(rb));
			Info_lb.Content = string.Format("Znalezione wersje: {0}", radioButtonsList.Count);
		}

		void CleanGUI()
		{
			radioButtonsList.Clear();
			StackPanel1.Children.Clear();
			Info_lb.Content = "";
		}

		Task<string> GetStringPage(string www)
		{
			return Task.Run(() =>
			Extensions.GetPage(www)
			);
		}

		async Task GetAllVersions()
		{
			string page = await GetStringPage(movie.addressPortal);
			if (page is null)
				return;

			string[] lines = page.Split('"').Where(a => a.StartsWith(Global.longPrefix)).Distinct().ToArray();
			string[] versions = lines.Where(a => a.Contains("wersja") && a.Length < 55).Distinct().ToArray();

			if (versions.Length < 1)
			{
				string[] incomplete_versions = page.Split('"').Where(a => a.StartsWith("/video") && a.Contains("wersja") && a.Length < 35).Distinct().ToArray();
				versions = incomplete_versions.Select(a => a = Global.shortPrefix + a).ToArray();
				if (versions.Length < 1)
				{
					string filmCode = Extensions.GetAddressLastSequence(Address_portal_tb.Text);
					versions = lines.Where(a => a.StartsWith(Global.longPrefix) && a.Contains(filmCode) && a.Length < 55).Distinct().ToArray();
					if (versions.Length < 1)
						return;
				}
			}
			await GetCryptoVersions(versions);
		}

		async Task GetCryptoVersions(string[] versions)
		{
			string page = null;
			foreach (string one_version in versions)
			{
				page = await GetStringPage(one_version);
				string[] addresses = (page.Split('"').Where(x => x.Contains(Global.protocol)).Distinct()).ToArray();
				if (addresses.Length < 1)
					continue;
				string crypto_address = addresses.First().Replace("\\/", "/");
				string decode_address = Extensions.DecodeOne(crypto_address);
				string name = Extensions.SetVersionsNames(one_version);
				movie.versions.Add(new Version(name, one_version, crypto_address, decode_address));
			}
		}

		void MakeRadioButtons()
		{
			if (movie.versions.Count < 1)
				return;
			RadioButton radioButton;
			int margin_top = 10, margin = 5;
			IList<RadioButton> localRadioButtonsList = new List<RadioButton>();

			foreach (Version oneVersion in movie.versions)
			{
				radioButton = new RadioButton
				{
					Name = "_" + oneVersion.Name + "_radioButton",
					Content = oneVersion.Name,
					Margin = new Thickness(margin, margin_top, margin, margin)
				};
				if (movie.versions.Last() == oneVersion)
					radioButton.IsChecked = true;

				localRadioButtonsList.Add(radioButton);
			}
			radioButtonsList = localRadioButtonsList.Reverse().ToList();
		}

		private void Paste_bt_Click(object sender, RoutedEventArgs e)
		{
			Address_portal_tb.Text = Clipboard.GetText();
			CleanGUI();
		}

		private void Open_bt_Click(object sender, RoutedEventArgs e)
		{
			foreach (RadioButton rb in StackPanel1.Children)
			{
				if (rb.IsChecked.Value)
				{
					string temp = rb.Content.ToString();
					string film = movie.versions.Where(x => x.Name == temp).First().DecodeVersion;
					Extensions.OpenFilm(film, applicationSettings.Programs);
					return;
				}
			}
		}

		private void Copy_bt_Click(object sender, RoutedEventArgs e)
		{
			foreach (RadioButton rb in StackPanel1.Children)
			{
				if (rb.IsChecked.Value)
				{
					string rb_content = rb.Content.ToString();
					string film = movie.versions.Where(x => x.Name == rb_content).First().DecodeVersion;
					Clipboard.SetText(film);
					return;
				}
			}
		}
	}
}