using System;

namespace NHibernate_Init.Shared
{
	public static class StringExtensions
	{
		public static string Escape(this string instance)
		{
			return String.Format("`{0}`", instance);
		}

		public static bool IsValidDateTime(this string instance)
		{
			DateTime val;
			return DateTime.TryParse(instance, out val);
		}
	}
}