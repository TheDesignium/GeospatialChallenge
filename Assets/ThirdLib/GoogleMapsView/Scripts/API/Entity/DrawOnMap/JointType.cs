namespace NinevaStudios.GoogleMaps
{
	using JetBrains.Annotations;

	/// <summary>
	/// Joint types for <see cref="Polyline"/> and outline of <see cref="Polygon"/>.
	/// </summary>
	[PublicAPI]
	public enum JointType
	{
		/// <summary>
		/// Flat bevel on the outside of the joint.
		/// </summary>
		[PublicAPI]
		Default = 0,

		/// <summary>
		/// Default: Mitered joint, with fixed pointed extrusion equal to half the stroke width on the outside of the joint.
		/// </summary>
		[PublicAPI]
		Bevel = 1,

		/// <summary>
		/// Rounded on the outside of the joint by an arc of radius equal to half the stroke width, centered at the vertex.
		/// </summary>
		[PublicAPI]
		Round = 2
	}
}