// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnMapClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<LatLng> _onMapClick;

		public OnMapClickListenerProxy(Action<LatLng> onMapClick)
			: base("com.google.android.gms.maps.GoogleMap$OnMapClickListener")
		{
			_onMapClick = onMapClick;
		}

		[UsedImplicitly]
		public void onMapClick(AndroidJavaObject pointAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onMapClick(LatLng.FromAJO(pointAJO)));
		}
	}
}
