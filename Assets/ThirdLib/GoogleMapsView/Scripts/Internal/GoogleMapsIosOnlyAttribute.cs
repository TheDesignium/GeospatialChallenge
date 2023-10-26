namespace NinevaStudios.GoogleMaps.Internal
{
	using System;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	public class GoogleMapsIosOnlyAttribute : Attribute
	{
	}
}