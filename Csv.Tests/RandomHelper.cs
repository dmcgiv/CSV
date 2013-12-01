using System;
using System.Text;

namespace CsvTests
{
	using JetBrains.Annotations;

	public static class RandomHelper
	{

		[NotNull]
		private static readonly Random _random;

		static RandomHelper()
		{
			_random = new Random((int)DateTime.Now.Ticks);
		}


		[NotNull]
		public static string[] StringArray(int length)
		{
			var items = new string[length];

			for (int i = 0; i < length; i++)
			{
				items[i] = String();
			}

			return items;
		}

		[CanBeNull]
		public static string String()
		{
			var sb = new StringBuilder();

			int length = _random.Next(200);

			if (length == 1)
			{
				return null;
			}
			else if (length == 2)
			{
				return string.Empty;
			}

			length -= 2;

			for (int i = 0; i < length; i++)
			{

				int c = Int(1, 95);

				if (c == 95)
				{
					sb.Append("\r\n");
					continue;
				}

				sb.Append((char)(c+32));
			}

			return sb.ToString();
		}

		public static int Int(int min, int max)
		{
			return _random.Next(min, max);
		}
	}
}
