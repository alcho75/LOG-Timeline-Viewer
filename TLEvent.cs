using System.ComponentModel;

namespace CLTL
{
	public class TLEvent : INotifyPropertyChanged
	{

		public int ms { get; set; }
		public int Interval { get; set; }
		public string Description { get; set; } = "---";

		private bool searchMatch;
		public bool SearchMatch
		{
			get => searchMatch;
			set
			{
				searchMatch = value;
				PropertyChanged?.Invoke(this, new(nameof(SearchMatch)));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public static TLEvent? FromString(string line)
		{
			string[] fields = line.Split(']');
			if (fields.Length >= 2)
			{
				bool suc = int.TryParse(fields[0].Substring(1), out var value);
				if (suc)
				{
					TLEvent tle = new() { ms = value, Description = fields[1] };

					return tle;
				}
			}
			return null;
		}


	}
}