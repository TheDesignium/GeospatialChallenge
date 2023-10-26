namespace NinevaStudios.GoogleMaps
{
	using System;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// Defines a Bitmap image. For a marker, this class can be used to set the image of the marker icon. 
	/// For a ground overlay, it can be used to set the image to place on the surface of the earth.
	/// </summary>
	[PublicAPI]
	public sealed class ImageDescriptor
	{
		const string ImageDescriptorFactoryClass = "com.google.android.gms.maps.model.BitmapDescriptorFactory";
		const string ExpansionFileUtilsClass = "com.deadmosquitogames.gmaps.ExpansionFileUtils";

		[PublicAPI] public const float HUE_RED = 0.0F;

		[PublicAPI] public const float HUE_ORANGE = 30.0F;

		[PublicAPI] public const float HUE_YELLOW = 60.0F;

		[PublicAPI] public const float HUE_GREEN = 120.0F;

		[PublicAPI] public const float HUE_CYAN = 180.0F;

		[PublicAPI] public const float HUE_AZURE = 210.0F;

		[PublicAPI] public const float HUE_BLUE = 240.0F;

		[PublicAPI] public const float HUE_VIOLET = 270.0F;

		[PublicAPI] public const float HUE_MAGENTA = 300.0F;

		[PublicAPI] public const float HUE_ROSE = 330.0F;

		public enum ImageDescriptorType
		{
			Default,
			DefaultWithHue,
			AssetName,
			Texture2D
		}

		public string AssetName { get; }

		public Texture2D Texture { get; }

		public ImageDescriptorType DescriptorType { get; } = ImageDescriptorType.Default;

		public float Hue { get; }

		ImageDescriptor(float hue)
		{
			DescriptorType = ImageDescriptorType.DefaultWithHue;
			Hue = hue;
		}

		ImageDescriptor(string assetName)
		{
			DescriptorType = ImageDescriptorType.AssetName;
			AssetName = assetName;
		}

		ImageDescriptor(Texture2D texture2D)
		{
			DescriptorType = ImageDescriptorType.Texture2D;
			Texture = texture2D;
		}

		ImageDescriptor()
		{
		}

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> that refers to the default marker image.
		/// </summary>
		/// <returns>The marker image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor DefaultMarker() => new ImageDescriptor();

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> that refers to a colorization of the default marker image. For convenience, there is a predefined set of hue values. E.g. <see cref="HUE_RED"/> 
		/// </summary>
		/// <returns>The marker image descriptor.</returns>
		/// <param name="hue">The hue of the marker. Value must be greater or equal to 0 and less than 360.</param>
		[PublicAPI]
		public static ImageDescriptor DefaultMarker(float hue) => new ImageDescriptor(hue);

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> using the name of the image in the StreamingAssets directory. Must be full image name inside StreamingAssets folder e.g. "my-custom-marker.png"
		/// </summary>
		/// <param name="assetName">Asset name. Must be full image name inside StreamingAssets folder e.g. "my-custom-marker.png"</param>
		/// <returns>The image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor FromAsset([NotNull] string assetName)
		{
			if (string.IsNullOrEmpty(assetName))
			{
				throw new ArgumentException("Image name cannot be null or empty", nameof(assetName));
			}

			return new ImageDescriptor(assetName);
		}

		/// <summary>
		/// Creates a <see cref="ImageDescriptor"/> using the name of the passed <see cref="Texture2D"/> object. The texture must be readable.
		/// </summary>
		/// <param name="texture2D">Texture.</param>
		/// <returns>The image descriptor.</returns>
		[PublicAPI]
		public static ImageDescriptor FromTexture2D([NotNull] Texture2D texture2D)
		{
			if (texture2D == null)
			{
				throw new ArgumentNullException(nameof(texture2D));
			}

			if (!texture2D.isReadable)
			{
				throw new ArgumentException($"Texture '{texture2D.name}' must be readable. See Read/Write Enabled setting in the inspector.");
			}

			return new ImageDescriptor(texture2D);
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			switch (DescriptorType)
			{
				case ImageDescriptorType.AssetName:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						try
						{
							if (IsAppBinarySplit)
							{
								var bitmap = ExpansionFileUtilsClass.AJCCallStaticOnce<AndroidJavaObject>("getBitmap", JniToolkitUtils.Activity, AssetName);
								return c.CallStaticAJO("fromBitmap", bitmap);
							}

							return c.CallStaticAJO("fromAsset", AssetName);
						}
						catch (Exception e)
						{
							Debug.LogError("Failed to load bitmap from expansion file: " + AssetName);
							Debug.LogException(e);
							return c.CallStaticAJO("defaultMarker");
						}
					}
				case ImageDescriptorType.DefaultWithHue:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("defaultMarker", Hue);
					}
				case ImageDescriptorType.Texture2D:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("fromBitmap", Texture.Texture2DToAndroidBitmap());
					}
				default:
					using (var c = new AndroidJavaClass(ImageDescriptorFactoryClass))
					{
						return c.CallStaticAJO("defaultMarker");
					}
			}
		}

		public static bool IsAppBinarySplit => Application.streamingAssetsPath.Contains("/obb/");
	}
}