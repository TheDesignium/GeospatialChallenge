using System.Collections.Generic;

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using UnityEngine;

	public static class GoogleMapUtils
	{
		public static bool IsAndroid => Application.platform == RuntimePlatform.Android;

		public static bool IsIos => Application.platform == RuntimePlatform.IPhonePlayer;

		public static bool IsNotAndroid => !IsAndroid;

		public static bool IsNotIosRuntime => !IsIos;

		public static bool IsPlatformSupported => IsAndroid || IsIos;

		public static bool IsPlatformNotSupported => !IsPlatformSupported;

		public static bool IsZero(this IntPtr intPtr)
		{
			return intPtr == IntPtr.Zero;
		}

		public static bool IsNonZero(this IntPtr intPtr)
		{
			return intPtr != IntPtr.Zero;
		}
		
		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (!(instanceHandle.Target is T))
			{
				throw new InvalidCastException("Failed to cast IntPtr");
			}

			var castedTarget = (T) instanceHandle.Target;
			return castedTarget;
		}
		
		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public static string ToFullStreamingAssetsPath(this string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				return null;
			}

			return Path.Combine(Application.streamingAssetsPath, fileName);
		}

		public static void AddImage(Dictionary<string, object> result, ImageDescriptor imageDescriptor)
		{
			if (imageDescriptor == null)
			{
				return;
			}

			switch (imageDescriptor.DescriptorType)
			{
				case ImageDescriptor.ImageDescriptorType.Default:
					break;
				case ImageDescriptor.ImageDescriptorType.DefaultWithHue:
					result["iconHue"] = imageDescriptor.Hue;
					break;
				case ImageDescriptor.ImageDescriptorType.AssetName:
					result["icon"] = imageDescriptor.AssetName.ToFullStreamingAssetsPath();
					result["iconAssetName"] = imageDescriptor.AssetName;
					result["isObbSplit"] = ImageDescriptor.IsAppBinarySplit;
					break;
				case ImageDescriptor.ImageDescriptorType.Texture2D:
					result["iconBytes"] = Convert.ToBase64String(imageDescriptor.Texture.EncodeToPNG());
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}