namespace NinevaStudios.GoogleMaps
{
	using JetBrains.Annotations;

	/// <inheritdoc />
	/// <summary>
	/// Cap that is squared off exactly at the start or end vertex of a <see cref="T:NinevaStudios.GoogleMaps.Polyline" /> with solid stroke pattern, equivalent to having no additional cap beyond the start or end vertex. 
	/// This is the default cap type at start and end vertices of Polylines with solid stroke pattern.
	/// Refer to https://developers.google.com/android/reference/com/google/android/gms/maps/model/ButtCap for more detailed documentation
	/// </summary>
	[PublicAPI]
	public sealed class ButtCap : Cap
	{

	}
}