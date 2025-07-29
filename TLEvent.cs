using System.ComponentModel;

namespace CLTL
{
	public class TLEvent : INotifyPropertyChanged
	{

		private int _ms;
		public int ms
		{
			get => _ms;
			set
			{
				_ms = value;
				PropertyChanged?.Invoke(this, new(nameof(ms)));
			}
		}

		private int interval;
		public int Interval
		{
			get => interval;
			set
			{
				interval = value;
				PropertyChanged?.Invoke(this, new(nameof(Interval)));
			}
		}

		public bool IsIntervalNegative { get => (Interval < 0); }

		private string description = "---";
		public string Description
		{
			get => description;
			set
			{
				description = value;
				PropertyChanged?.Invoke(this, new(nameof(Description)));
			}
		}

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

		internal void Clear()
		{
			ms = 0;
			Interval = 0;
			Description = "---";
		}
	}
}