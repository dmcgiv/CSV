

namespace Csv
{
	using System;
	using System.IO;

	using JetBrains.Annotations;

	public class CsvWriter : IDisposable
	{
		[NotNull]
		private readonly TextWriter _writer;

		[NotNull]
		private readonly char[] _charactersTheRequireValueToBeQuoted;


		private const char Quote = '\"';

		private const string NewLine = "\r\n";

		private readonly char _delimiter = ',';

		private int _columnCount = -1;

		private bool _headerWriten = false;

		public CsvWriter([NotNull]TextWriter writer, char delimiter = ',')
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			if (delimiter == Quote)
			{
				throw new ArgumentException("Delimiter cannot be a double quote.", "delimiter");
			}

			_writer = writer;
			_delimiter = delimiter;
			this._charactersTheRequireValueToBeQuoted = new char[] { _delimiter, '\n', '\r', Quote };

			Reset();
		}


		void Reset()
		{
			_headerWriten = false;
			_columnCount = -1;
		}



		/// <summary>
		/// When true string with witespace characters at start or end of value are wrapped in quotes.
		/// 
		/// http://en.wikipedia.org/wiki/Comma-separated_values
		/// In some CSV implementations, leading and trailing spaces and tabs are trimmed. This practice is controversial, and does not accord with RFC 4180, which states "Spaces are considered part of a field and should not be ignored.".
		/// </summary>
		public bool QuoteLeadingAndTrailingWhiteSpace { get; set; }


		/// <summary>
		/// When true empty string values are wrapped in double quotes.
		/// 
		/// http://programmers.stackexchange.com/a/65178/14723
		/// Not part of RFC 4180 but may be required for some parser implementations.
		/// </summary>
		public bool QuoteEmptyValues { get; set; }

		public void WriteHeaders([NotNull]string[] headers)
		{
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}

			if (headers.Length == 0)
			{
				throw new ArgumentException("Headers is empty", "headers");
			}

			if (_headerWriten)
			{
				throw new InvalidOperationException("Headers have already been written.");
			}

			_headerWriten = true;
			_columnCount = headers.Length;


			if (string.IsNullOrEmpty(headers[0]))
			{
				throw new ArgumentException(string.Format("Header with the index {0} is null or empty", 0), "headers");
			}

			Write(headers[0]);

			for (int i = 1; i < headers.Length; i++)
			{
				if (string.IsNullOrEmpty(headers[i]))
				{
					throw new ArgumentException(string.Format("Header with the index {0} is null or empty", i), "headers");
				}

				_writer.Write(_delimiter);
				Write(headers[i]);
			}


		}

		public void WriteRow([NotNull]string[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}

			if (values.Length == 0)
			{
				throw new ArgumentException("Headers is empty", "values");
			}

			if (_columnCount == -1)
			{
				// this is the first row
				_columnCount = values.Length;
			}
			else if (_columnCount != values.Length)
			{
				throw new ArgumentException(string.Format("Expecting {0} values, only {1} given.", _columnCount, values.Length), "values");
			}
			else
			{
				_writer.Write(NewLine);
			}

			Write(values[0]);

			for (int i = 1; i < values.Length; i++)
			{
				_writer.Write(_delimiter);
				Write(values[i]);
			}

		}


		/// <summary>
		/// Writes a single cell value
		/// </summary>
		void Write([CanBeNull]string value)
		{
			if (value == null)
			{
				// ignore - do not write anything
				return;
			}

			if (value.Length == 0)
			{
				if (this.QuoteEmptyValues)
				{
					_writer.Write("\"\"");
				}

				return;
			}


			int start = value.IndexOfAny(this._charactersTheRequireValueToBeQuoted);
			if (start != -1)
			{
				// wrap value in quotes
				_writer.Write(Quote);

				int end = value.IndexOf(Quote, start);

				if (end != -1)
				{
					// value contains quotes these need to be escaped by adding an additional quote
					_writer.Write(value.Substring(0, start));

					do
					{
						_writer.Write(value.Substring(start, end - start + 1));
						_writer.Write(Quote);

						start = end + 1;

						end = value.IndexOf(Quote, start);
					}
					while (end != -1);

					_writer.Write(value.Substring(start));

				}
				else
				{
					_writer.Write(value);
				}

				_writer.Write(Quote);

			}
			else
			{

				if (this.QuoteLeadingAndTrailingWhiteSpace
					&& (value[0] == ' ' || value[0] == '\t' || value[value.Length - 1] == ' ' || value[value.Length - 1] == '\t'))
				{
					_writer.Write(Quote);
					_writer.Write(value);
					_writer.Write(Quote);
				}
				else
				{
					_writer.Write(value);
				}


			}

		}


		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			if (disposing)
			{
				_writer.Flush();
				_writer.Dispose();
			}

			disposed = true;
		}
	}
}
