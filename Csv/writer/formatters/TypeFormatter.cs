namespace Csv
{
	using System;
	using System.Collections.Generic;

	using JetBrains.Annotations;

	public class TypeFormatter : IValueFormatter
	{
		[NotNull]
		private readonly Dictionary<Type, Func<object, string>> _formatters;

		public TypeFormatter( )
		{
			this._formatters = new Dictionary<Type, Func<object, string>>();
		}


		public string Format([CanBeNull] object value)
		{
			if(value == null)
			{
				return null;
			}

			Func<object, string> formatter;
			if (this._formatters.TryGetValue(value.GetType(), out formatter))
			{
				return formatter(value);
			}

			return value.ToString();
		}

		[NotNull]
		public TypeFormatter SetUpFormat<T>([NotNull] Func<T, string> format, string nullValue = null)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}

			var f = new Func<object, string>(
				o =>
					{
						if (o == null)
						{
							return nullValue;
						}

						var v = (T)o;

						return format(v);
					});

			this._formatters.Add(typeof(T), f);

			return this;
		}
	}
}