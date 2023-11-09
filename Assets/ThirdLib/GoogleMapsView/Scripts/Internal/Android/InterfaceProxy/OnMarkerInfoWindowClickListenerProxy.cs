using System;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	public class OnMarkerInfoWindowClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<Marker> _onMarkerOnfoWindowClick;

		public OnMarkerInfoWindowClickListenerProxy(Action<Marker> onMarkerOnfoWindowClick) : base(
			"com.google.android.gms.maps.GoogleMap$OnInfoWindowClickListener")
		{
			_onMarkerOnfoWindowClick = onMarkerOnfoWindowClick;
		}

		[UsedImplicitly]
		public void onInfoWindowClick(AndroidJavaObject circleAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onMarkerOnfoWindowClick(new Marker(circleAJO)));
		}
	}
}