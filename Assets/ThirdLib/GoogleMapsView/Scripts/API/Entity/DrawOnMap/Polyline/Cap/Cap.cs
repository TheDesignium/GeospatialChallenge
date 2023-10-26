namespace NinevaStudios.GoogleMaps
{
	using JetBrains.Annotations;
	using UnityEngine;

	/// <summary>
	/// Immutable cap that can be applied at the start or end vertex of a <see cref="Polyline"/>.
	/// 
	/// Refer to https://developers.google.com/android/reference/com/google/android/gms/maps/model/Cap for more detailed documentation
	/// </summary>
	[PublicAPI]
	public abstract class Cap
	{
		const string ButtCapClass = "com.google.android.gms.maps.model.ButtCap";
		const string CustomCapClass = "com.google.android.gms.maps.model.CustomCap";
		const string RoundCapClass = "com.google.android.gms.maps.model.RoundCap";
		const string SquareCapClass = "com.google.android.gms.maps.model.SquareCap";
		
		public AndroidJavaObject ToAJO()
		{
			AndroidJavaObject ajo = null;
			
			if (this is ButtCap)
			{
				ajo = new AndroidJavaObject(ButtCapClass);
			}
			else if (this is SquareCap)
			{
				ajo = new AndroidJavaObject(SquareCapClass);
			}
			else if (this is RoundCap)
			{
				ajo = new AndroidJavaObject(RoundCapClass);
			}
			else if (this is CustomCap)
			{
				var customCap = (CustomCap) this;
				ajo = new AndroidJavaObject(CustomCapClass, customCap.Image.ToAJO(), customCap.RefWidth);
			}

			return ajo;
		}
	}
}