

namespace Csv
{

	using System;
	using System.Linq;
	using JetBrains.Annotations;

	public class CsvObjectWriter
	{
		[NotNull]
		private readonly IValueFormatter _valueFormatter;

		[NotNull]
		private readonly CsvWriter _writer;

		public CsvObjectWriter([NotNull]CsvWriter writer)
			: this(writer, new DefaultValueFormatter())
		{
			
		}

		public CsvObjectWriter([NotNull] CsvWriter writer, [NotNull] IValueFormatter valueFormatter)
		{
			if (valueFormatter == null)
			{
				throw new ArgumentNullException("valueFormatter");
			}

			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			this._valueFormatter = valueFormatter;
			this._writer = writer;
		}




		public void WriteRow([NotNull] object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}

			_writer.WriteRow(values.Select(v => this._valueFormatter.Format(v)).ToArray());
		}
	}

	public interface IValueFormatter
	{
		string Format( object value);
	}
}
