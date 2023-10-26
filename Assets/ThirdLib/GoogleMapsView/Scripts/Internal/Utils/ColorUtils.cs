using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps.Internal
{
	using System.Collections.Generic;
	using MiniJSON;
	using UnityEngine;

	public static class ColorUtils
	{
		public static Color RandomColor()
		{
			return new Color(Random.value, Random.value, Random.value);
		}
		
		public static int ToAndroidColor(this Color32 color32)
		{
			Color color = color32;
			return ToAndroidColor(color);
		}

		public static int ToAndroidColor(this Color color)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return 0;
			}

			Color32 c = color;
			return AndroidColor(c.a, c.r, c.g, c.b);
		}

		public static Dictionary<string, object> ToDictionary(this Color color)
		{
			var result = new Dictionary<string, object>
			{
				["r"] = color.r,
				["g"] = color.g,
				["b"] = color.b,
				["a"] = color.a
			};
			return result;
		}

		public static Color ColorFromJson(this string colorJson)
		{
			var dic = Json.Deserialize(colorJson) as Dictionary<string, object>;
			float r = dic.GetFloat("r");
			float g = dic.GetFloat("g");
			float b = dic.GetFloat("b");
			float a = dic.GetFloat("a");
			return new Color(r, g, b, a);
		}

		static int AndroidColor(int alpha,
			int red,
			int green,
			int blue)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return 0;
			}

			using (var c = new AndroidJavaClass("android.graphics.Color"))
			{
				return c.CallStaticInt("argb", alpha, red, green, blue);
			}
		}

		public static Color ToUnityColor(this int aCol)
		{
			Color32 c = Color.clear;
			c.b = (byte) (aCol & 0xFF);
			c.g = (byte) ((aCol >> 8) & 0xFF);
			c.r = (byte) ((aCol >> 16) & 0xFF);
			c.a = (byte) ((aCol >> 24) & 0xFF);
			return c;
		}

		public static float[] ToFloatArr(this Color c)
		{
			return new[] {c.r, c.g, c.b, c.a};
		}

		public static Color ToColor(this float[] c)
		{
			return new Color(c[0], c[1], c[2], c[3]);
		}
	}
}