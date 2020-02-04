using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MovierGit.Classes;

namespace MovierGit
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			InitialApplicationSettings();
		}
		ApplicationSettings _applicationSettings;
		List<RadioButton> _radioButtonsList = new List<RadioButton>();
		Film _movie;

		public void InitialApplicationSettings()
		{
			_applicationSettings = Extensions.Deserialize(Global.Settings);
			_applicationSettings = _applicationSettings ?? new ApplicationSettings(Global.Chrome86);
		}

		private async void Go_bt_Click(object sender, RoutedEventArgs e)
		{
			var portal = Address_portal_tb.Text;
			if (string.IsNullOrEmpty(portal) || portal.Length < 27)
				return;
			CleanGui();
			Go_bt.IsEnabled = false;
			_movie = new Film(portal);
			await GetAllVersions();
			MakeRadioButtons();
			ShowAllVersions();
			Go_bt.IsEnabled = true;
		}

		private void ShowAllVersions()
		{
			if (_radioButtonsList.Count < 1)
			{
				Info_lb.Content = Properties.Resources.DownloadingError;
				return;
			}
			_radioButtonsList.ForEach(rb => StackPanel1.Children.Add(rb));
			Info_lb.Content = $"Znalezione wersje: {_radioButtonsList.Count}";
		}

		private void CleanGui()
		{
			_radioButtonsList.Clear();
			StackPanel1.Children.Clear();
			Info_lb.Content = "";
		}

		private static Task<string> GetStringPage(string www)
		{
			return Task.Run(() =>
			Extensions.GetPage(www)
			);
		}

		private async Task GetAllVersions()
		{
			var page = await GetStringPage(_movie.AddressPortal);
			if (page == null)
				return;

			var lines = page.Split('"').Where(a => a.StartsWith(Global.LongPrefix)).Distinct().ToArray();
			var allVersions = lines.Where(a => a.Contains("wersja") && a.Length < 55).Distinct().ToArray();

			if (allVersions.Length < 1)
			{
				var incompleteVersions = page.Split('"')
					.Where(a => a.StartsWith("/video") && a.Contains("wersja") && a.Length < 35).Distinct().ToArray();
				allVersions = incompleteVersions.Select(a => a = Global.ShortPrefix + a).ToArray();
				if (allVersions.Length < 1)
				{
					var filmCode = Extensions.GetAddressLastSequence(Address_portal_tb.Text);
					allVersions = lines.Where(a => a.StartsWith(Global.LongPrefix) && a.Contains(filmCode) && a.Length < 55).Distinct().ToArray();
					if (allVersions.Length < 1)
						return;
				}
			}
			await GetCryptoVersions(allVersions);
		}

		private async Task GetCryptoVersions(string[] versions)
		{
			foreach (var oneVersion in versions)
			{
				var page = await GetStringPage(oneVersion);
				var addresses = page.Split('"').Where(x => x.Contains(Global.Protocol)).Distinct().ToArray();
				if (addresses.Length < 1)
					continue;
				var cryptoAddress = addresses.First().Replace("\\/", "/");
				var decodeOneVersion = Extensions.DecodeOneVersion(cryptoAddress);
				var name = Extensions.SetVersionsNames(oneVersion);
				_movie.Versions.Add(new Version(name, oneVersion, cryptoAddress, decodeOneVersion));
			}
		}

		private void MakeRadioButtons()
		{
			if (_movie.Versions.Count < 1)
				return;
			int marginTop = 10, margin = 5;
			IList<RadioButton> localRadioButtonsList = new List<RadioButton>();

			foreach (Version oneVersion in _movie.Versions)
			{
				var radioButton = new RadioButton
				{
					Name = "_" + oneVersion.Name + "_radioButton",
					Content = oneVersion.Name,
					Margin = new Thickness(margin, marginTop, margin, margin)
				};
				if (_movie.Versions.Last() == oneVersion)
					radioButton.IsChecked = true;

				localRadioButtonsList.Add(radioButton);
			}
			_radioButtonsList = localRadioButtonsList.Reverse().ToList();
		}

		private void Paste_bt_Click(object sender, RoutedEventArgs e)
		{
			Address_portal_tb.Text = Clipboard.GetText();
			CleanGui();
		}

		private void Open_bt_Click(object sender, RoutedEventArgs e)
		{
			foreach (RadioButton radioButton in StackPanel1.Children)
			{
				if (radioButton.IsChecked != null && radioButton.IsChecked.Value)
				{
					var film = _movie.Versions.First(x => x.Name == radioButton.Content.ToString()).DecodeVersion;
					Extensions.OpenFilm(film, _applicationSettings.Programs);
					return;
				}
			}
		}

		private void Copy_bt_Click(object sender, RoutedEventArgs e)
		{
			foreach (RadioButton radioButton in StackPanel1.Children)
			{
				if (radioButton.IsChecked != null && radioButton.IsChecked.Value)
				{
					var film = _movie.Versions.First(x => x.Name == radioButton.Content.ToString()).DecodeVersion;
					Clipboard.SetText(film);
					return;
				}
			}
		}
	}
}