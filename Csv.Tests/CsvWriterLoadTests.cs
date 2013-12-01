namespace CsvTests
{
	using System;
	using System.Diagnostics;
	using System.IO;

	using Csv;

	using NUnit.Framework;

	[TestFixture]
	public class CsvWriterLoadTests
	{

		public static void Main()
		{
			new CsvWriterLoadTests().WriteLoad();
		}

		[Test]
		public void WriteLoad()
		{
			var timer = Stopwatch.StartNew();
			const int rows = 50000;

			//var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CsvWriter.csv");
			//using (var fileStream = new FileStream(testFile, FileMode.Create))

			using (var fileStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(fileStream))
			using (var csvWriter = new CsvWriter(streamWriter))
			{

				var row = RandomHelper.StringArray(20);
				for (int i = 0; i < rows; i++)
				{
					csvWriter.WriteRow(row);
				}

			}

			timer.Stop();

			Console.WriteLine("{0} in {1}",rows, timer.Elapsed);
		}
	}
}
