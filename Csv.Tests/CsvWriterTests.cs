namespace CsvTests
{
	using System;
	using System.IO;
	using System.Linq;

	using Csv;

	using JetBrains.Annotations;

	using NUnit.Framework;


	/// <summary>
	/// see http://tools.ietf.org/html/rfc4180
	/// </summary>
	[TestFixture]
	public class CsvWriterTests
	{


		[Test]
		public void Empty_String_Empty_Quotes()
		{
			var values = new string[] { "", null };

			var text = GetString(writer => writer.WriteRow(values));

			Assert.AreEqual("\"\",", text);


			values = new string[] {  null , ""};

			text = GetString(writer => writer.WriteRow(values));

			Assert.AreEqual(",\"\"", text);
		}

		[Test]
		public void Multiple_Rows_With_No_Headers()
		{

			var values = new string[] { "aaa", "bbb", "ccc" };

			var text = GetString(
				writer =>
					{
						writer.WriteRow(values);
						writer.WriteRow(values);
					});

			Assert.AreEqual("aaa,bbb,ccc\r\naaa,bbb,ccc", text);
		}


		[Test]
		public void Multiple_Rows_With_Headers()
		{
			var headers = new string[] { "a", "b", "c" };
			var values = new string[] { "aaa", "bbb", "ccc" };

			var text = GetString(
				writer =>
				{
					writer.WriteHeaders(headers);
					writer.WriteRow(values);
					writer.WriteRow(values);
				});

			Assert.AreEqual("a,b,c\r\naaa,bbb,ccc\r\naaa,bbb,ccc", text);
		}

		[Test]
		public void Headers_Multiple_No_Encoding()
		{

			var headers = new string[] { "aaa", "bbb", "ccc" };

			var text = GetString(writer => writer.WriteHeaders(headers));

			Assert.AreEqual("aaa,bbb,ccc", text);

		}


		[Test]
		public void Value_Starting_With_White_Space_No_Quoting()
		{

			var headers = new string[] { " a", "b ", " c " };

			var text = GetString(writer => writer.WriteHeaders(headers));

			Assert.AreEqual( " a,b , c ", text);
		}


		[Test]
		public void Value_Starting_With_White_Space_Quoting()
		{

			var headers = new string[] { " a", "b ", " c " };

			var text = GetString(
				writer =>
					{
						writer.QuoteLeadingAndTrailingWhiteSpace = true;
						writer.WriteHeaders(headers);
					});

			Assert.AreEqual("\" a\",\"b \",\" c \"", text);
		}


		[Test]
		public void Headers_Multiple_With_Encoding()
		{

			var headers = new string[] { "a,aa", @"b""bb", @"c
cc" };

			var text = GetString(writer => writer.WriteHeaders(headers));

			Assert.AreEqual(@"""a,aa"",""b""""bb"",""c
cc""", text);

		}

		[Test]
		public void Headers_Single_No_Encoding()
		{

			var headers = new string[] { "aaa"};
			//var values = new string[] { "xxx", "yyy", "zzz" };

			var text = GetString(writer => writer.WriteHeaders(headers));

			Assert.AreEqual("aaa", text);

		}


		[Test]
		public void Headers_Single_With_Encoding()
		{

			var headers = new string[] { @"a,aab""bbc
cc" };
			//var values = new string[] { "xxx", "yyy", "zzz" };

			var text = GetString(writer => writer.WriteHeaders(headers));

			Assert.AreEqual(@"""a,aab""""bbc
cc""", text);

		}



		[Test]
		public void Empty_Header_Value_Fails()
		{
			var headers = new string[] { "" };

			GetString(
				writer =>
				{
					Assert.Throws<ArgumentException>(() => writer.WriteHeaders(headers));

				});
		}


		[Test]
		public void Null_Header_Value_Fails()
		{
			var headers = new string[] { null };

			GetString(
				writer =>
				{
					Assert.Throws<ArgumentException>(() => writer.WriteHeaders(headers));

				});
		}

		[Test]
		public void Quote()
		{

			// to make test easier to read replace encoded double quote qith a 'q'
			var headers = new string[] { "q", "qq","q q",  " q q " }.Select(x=>x.Replace('q','\"')).ToArray();

			var text = GetString(writer => writer.WriteHeaders(headers));

			string expected = "qqqq,qqqqqq,qqq qqq,q qq qq q".Replace('q', '\"');
			Assert.AreEqual(expected, text);
		}

		[Test]
		public void Each_Row_Column_Count_Must_Be_Equal()
		{
			
			var headers = new string[] { "aaa", "bbb", "ccc" };
			var rowTooMany = new string[] { "aaa", "bbb", "ccc", "ddd" };
			var rowTooFew = new string[] { "aaa", "bbb"};


			// writing headers
			GetString(
				writer =>
					{
						writer.WriteHeaders(headers);

						Assert.Throws<ArgumentException>(() => writer.WriteRow(rowTooMany));

						Assert.Throws<ArgumentException>(() => writer.WriteRow(rowTooFew));
					});

			// not writing headers
			GetString(
				writer =>
				{
					writer.WriteRow(headers);

					Assert.Throws<ArgumentException>(() => writer.WriteRow(rowTooMany));

					Assert.Throws<ArgumentException>(() => writer.WriteRow(rowTooFew));
				});
		}


		[Test]
		public void Can_Only_Write_Headers_once()
		{
			var headers = new string[] { "aaa", "bbb", "ccc" };

			GetString(
				writer =>
				{
					writer.WriteHeaders(headers);

					Assert.Throws<InvalidOperationException>(() => writer.WriteHeaders(headers));

				});
		}



		[NotNull]
		static string GetString([NotNull]Action<CsvWriter> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			using (var memStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memStream))
			using (var csvWriter = new CsvWriter(streamWriter))
			{
				action(csvWriter);
				
				streamWriter.Flush();

				memStream.Position = 0;
				var sr = new StreamReader(memStream);
				return sr.ReadToEnd();

			}
		}
	}
}
