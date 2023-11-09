namespace NinevaStudios.GoogleMaps
{
	using JetBrains.Annotations;

	/// <summary>
	/// Google map type.
	/// </summary>
	[PublicAPI]
	public enum GoogleMapType
	{
		/// <summary>
		/// Satellite maps with a transparent layer of major streets.
		/// </summary>
		[PublicAPI]
		Hybrid = 4,

		/// <summary>
		/// No base map tiles.
		/// </summary>
		[PublicAPI]
		None = 0,

		/// <summary>
		/// Basic maps.
		/// </summary>
		[PublicAPI]
		Normal = 1,

		/// <summary>
		/// Satellite maps with no labels.
		/// </summary>
		[PublicAPI]
		Satellite = 2,

		/// <summary>
		/// Terrain maps.
		/// </summary>
		[PublicAPI]
		Terrain = 3,
	}
}