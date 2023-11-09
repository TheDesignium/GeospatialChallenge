namespace NinevaStudios.GoogleMaps
{
	using JetBrains.Annotations;

	/// <inheritdoc />
	/// <summary>
	/// Cap that is a semicircle with radius equal to half the stroke width, centered at the start or end vertex of a <see cref="T:NinevaStudios.GoogleMaps.Polyline" /> with solid stroke pattern.
	/// See: https://developers.google.com/android/reference/com/google/android/gms/maps/model/RoundCap
	/// </summary>
	[PublicAPI]
	public sealed class RoundCap : Cap
	{
	}
}