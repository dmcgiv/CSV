

namespace CsvTests
{
	using System;
	using System.IO;

	using Csv;
	using Csv.writer;

	using JetBrains.Annotations;

	using NUnit.Framework;

	[TestFixture]
	public class CsvTypeWriterTests
	{

		public class TestType
		{
			public DateTime DateTime { get; set; }
			public decimal Decimal { get; set; }
		}


		[Test]
		public void Test()
		{

			var rows = new TestType[]
				           {
					           new TestType { DateTime = new DateTime(2013, 11, 30), Decimal = 12.345678m },
					           new TestType { DateTime = new DateTime(2013, 12, 29), Decimal = 45.321m }
				           };

			var formatter = new CsvTypeWriterConfig<TestType>();
			formatter
				.SetUpColumnFormat("Date Col", type => type.DateTime.ToString("dd-MMM-yyyy"))
				.SetUpColumnFormat("Dec", type => type.Decimal.ToString("F4"));


			var text = GetString<TestType>(
				(writer, typeWriter) =>
					{
						typeWriter.Writeheaders();

						foreach (var row in rows)
						{
							typeWriter.WriteRow(row);
						}
						
					}, formatter);


			Assert.AreEqual("Date Col,Dec\r\n30-Nov-2013,12.3457\r\n29-Dec-2013,45.3210", text);
		}




		[NotNull]
		static string GetString<T>([NotNull]Action<CsvWriter, CsvTypeWriter<T>> action, [NotNull] ITypeFormatter<T> formatter)
			where T : class
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}

			using (var memStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memStream))
			using (var csvWriter = new CsvWriter(streamWriter))
			{
				var objWriter = new CsvTypeWriter<T>(csvWriter, formatter);
				action(csvWriter, objWriter);

				streamWriter.Flush();

				memStream.Position = 0;
				var sr = new StreamReader(memStream);
				return sr.ReadToEnd();

			}
		}
	}
}
