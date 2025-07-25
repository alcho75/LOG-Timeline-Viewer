using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CLTL
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private List<int> searchList = new();
		public ObservableCollection<TLEvent> TLEventsStorage { get; set; } = [];
		public ObservableCollection<TLEvent> TLEvents { get; set; } = [];
		public string SearchResults { get => $"{SearchIndex} / {SearchCount}"; }
		public int SearchIndex { get; set; }
		public int SearchCount { get; set; }

		const int TLEventControlCount = 50;

		public MainWindow()
		{
			for (int i = 0; i < TLEventControlCount; i++) TLEvents.Add(new());

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Reload();
		}

		private void Reload()
		{
			LoadLogs();
			slider.Focus();

			if(searchBox.Text != "")
			{
				Search
			}
		}

		TLEvent? prvTLE = null;

		public event PropertyChangedEventHandler? PropertyChanged;

		void LoadFile(string filePath)
		{
			if (!File.Exists(filePath)) return;

			foreach (string line in File.ReadAllLines(filePath))
			{
				TLEvent? tle = TLEvent.FromString(line);
				if (tle != null)
				{
					if (prvTLE != null)
					{
						prvTLE.Interval = tle.ms - prvTLE.ms;
					}
					TLEventsStorage.Add(tle);
					prvTLE = tle;
				}
			}
		}

		void LoadLogs()
		{
			OpenFolderDialog ofd = new OpenFolderDialog();
			bool? ok = ofd.ShowDialog();
			if (ok == null) return;
			if (ok == true)
			{
				TLEventsStorage.Clear();

				List<string> files = [.. Directory.GetFiles(ofd.FolderName, "*.log")];
				files.Sort();

				foreach (string path in files) LoadFile(path);

				Title += $" - {TLEventsStorage.Count} lines";

				slider.Value = 0;
				if (TLEventsStorage.Count > TLEventControlCount)
				{
					slider.IsEnabled = true;
					slider.Maximum = TLEventsStorage.Count - TLEventControlCount;
					slider.SmallChange = 1;
					slider.LargeChange = slider.SmallChange * TLEventControlCount;
				}
				else slider.IsEnabled = false;

				SetEventsView(0);
			}
		}

		private void SetEventsView(int startIndex)
		{
			for (int i = 0; i < TLEventControlCount; i++)
			{
				int index = i + startIndex;
				if (index < TLEventsStorage.Count)
				{
					TLEvents[i] = TLEventsStorage[index];
				}
				else TLEvents[i].Clear();
			}
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SetEventsView((int)e.NewValue);
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Search();
		}

		private void Search()
		{
			SearchCount = Serach(searchBox.Text);
			PropertyChanged?.Invoke(this, new(nameof(SearchResults)));
		}

		private int Serach(string text)
		{
			SearchIndex = 0;
			int sc = 0;
			int i = 0;
			searchList.Clear();
			foreach(TLEvent tle in TLEventsStorage)
			{
				tle.SearchMatch = false;
				if (text != "")
				{
					if (tle.Description.Contains(text) || (tle.Interval.ToString().Contains(text)))
					{
						tle.SearchMatch = true;
						searchList.Add(i);
						sc++;
					}
				}
				i++;
			}
			return sc;
		}

		private void searchBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (SearchCount == 0) return;

				if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				{
					if (SearchIndex < SearchCount) SearchIndex++;
					else SearchIndex = 1;
				}
				else
				{
					if (SearchIndex > 1) SearchIndex--;
					else SearchIndex = SearchCount;
				}
				PropertyChanged?.Invoke(this, new(nameof(SearchResults)));
				if (SearchIndex > 0)
				{
					UpdateSlider(searchList[SearchIndex - 1]);
				}
			}
		}

		private void UpdateSlider(int startIndex)
		{
			slider.Value = startIndex;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Reload();
		}
	}
}
