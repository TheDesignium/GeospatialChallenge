using JetBrains.Annotations;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Reason why camera moved
	/// </summary>
	[PublicAPI]
	public enum CameraMoveReason
	{
		/// <summary>
		/// Non-gesture animation initiated in response to user actions.
		/// </summary>
		ApiAnimation = 2,
		
		/// <summary>
		/// Developer initiated animation.
		/// </summary>
		DeveloperAnimation = 3,
		
		/// <summary>
		/// Camera motion initiated in response to user gestures on the map.
		/// </summary>
		Gesture = 1
	}
}