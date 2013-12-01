

namespace CsvTests
{
	using System;
	using System.IO;

	using Csv;

	using JetBrains.Annotations;

	using NUnit.Framework;

	[TestFixture]
	public class CsvObjectWriterTests
	{

		[Test]
		public void A()
		{
			var config = new TypeFormatter();

			config
				.SetUpFormat<DateTime>(x => x.ToString("dd-MMM-yyyy"))
				.SetUpFormat<decimal>(x => x.ToString("F4"));


			var values = new object[] { new DateTime(2013, 11, 23), 23.4565473m, 23, "abc" };

			var text = GetString(
				(csvWriter, objWriter) =>
					{
						objWriter.WriteRow(values);
					}, config);

			Assert.AreEqual("23-Nov-2013,23.4565,23,abc", text);

		}


		[NotNull]
		static string GetString([NotNull]Action<CsvWriter, CsvObjectWriter> action, IValueFormatter formatter = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			using (var memStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memStream))
			using (var csvWriter = new CsvWriter(streamWriter))
			{
				var objWriter = new CsvObjectWriter(csvWriter, formatter ?? new DefaultValueFormatter());
				action(csvWriter, objWriter);

				streamWriter.Flush();

				memStream.Position = 0;
				var sr = new StreamReader(memStream);
				return sr.ReadToEnd();

			}
		}
	}
}
