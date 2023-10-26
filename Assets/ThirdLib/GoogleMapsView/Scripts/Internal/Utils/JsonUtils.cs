namespace NinevaStudios.GoogleMaps.Internal
{
	using System.Collections.Generic;

	public static class JsonUtils
	{
		public static string GetStr(this Dictionary<string, object> dic, string key)
		{
			return dic.ContainsKey(key) ? (string) dic[key] : null;
		}

		public static int GetInt(this Dictionary<string, object> dic, string key)
		{
			return dic.ContainsKey(key) ? (int) (long) dic[key] : 0;
		}

		public static double GetDouble(this Dictionary<string, object> dic, string key)
		{
			if (!dic.ContainsKey(key))
			{
				return 0f;
			}
			
			if (dic[key] is long)
			{
				return dic.GetInt(key);
			}

			return (double) dic[key];
		}

		public static float GetFloat(this Dictionary<string, object> dic, string key)
		{
			return (float) dic.GetDouble(key);
		}
	}
}