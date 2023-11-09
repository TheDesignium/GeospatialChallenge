// ReSharper disable InconsistentNaming
namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnMapLongClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<LatLng> _onLongMapClick;

		public OnMapLongClickListenerProxy(Action<LatLng> onLongMapClick)
			: base("com.google.android.gms.maps.GoogleMap$OnMapLongClickListener")
		{
			_onLongMapClick = onLongMapClick;
		}

		[UsedImplicitly]
		public void onMapLongClick(AndroidJavaObject pointAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onLongMapClick(LatLng.FromAJO(pointAJO)));
		}
	}
}
