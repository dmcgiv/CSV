using System;
using System.Collections.Generic;
using System.Linq;


namespace Csv.writer
{
	using JetBrains.Annotations;

	public class CsvTypeWriter<T>
		where T : class
	{

		[NotNull] 
		private readonly ITypeFormatter<T> _formatter;

		[NotNull] 
		private readonly CsvWriter _writer;


		public CsvTypeWriter([NotNull] CsvWriter writer, [NotNull] ITypeFormatter<T> formatter )
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}
			this._formatter = formatter;
			this._writer = writer;
		}


		public void WriteRow([NotNull] T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			var values = _formatter.Format(value);

			_writer.WriteRow(values);
		}

		public void Writeheaders()
		{
			_writer.WriteHeaders(_formatter.Headers);
		}
	}


	public class CsvTypeWriterConfig<T> : ITypeFormatter<T>
		where T : class
	{
		[NotNull] 
		private readonly List<string> _headers;

		[NotNull] 
		private readonly Dictionary<string, Func<T, string>> _columnFormatters;

		public CsvTypeWriterConfig()
		{
			this._headers = new List<string>();

			_columnFormatters = new Dictionary<string, Func<T, string>>();
		}

		public CsvTypeWriterConfig<T> SetUpColumnFormat([NotNull] string name, [NotNull] Func<T, string> format)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			if (format == null)
			{
				throw new ArgumentNullException("format");
			}

			_columnFormatters.Add(name, format);
			_headers.Add(name);
			return this;
		}

		public string[] Format(T item)
		{
			return _headers.Select(header => _columnFormatters[header](item)).ToArray();
		}

		public string[] Headers
		{
			get
			{
				return _headers.ToArray();
			}

		}
	}

	public interface ITypeFormatter<in T>
	{
		string[] Format(T item);

		string[] Headers { get; }
	}
}
