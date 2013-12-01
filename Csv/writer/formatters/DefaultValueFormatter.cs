namespace Csv
{
	public class DefaultValueFormatter : IValueFormatter
	{
		public string Format(object value)
		{
			return value == null ? null : value.ToString();
		}
	}
}