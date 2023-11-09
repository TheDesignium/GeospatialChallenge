// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnPolylineClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<Polyline> _onPolylineClick;

		public OnPolylineClickListenerProxy(Action<Polyline> onPolylineClick)
			: base("com.google.android.gms.maps.GoogleMap$OnPolylineClickListener")
		{
			_onPolylineClick = onPolylineClick;
		}

		[UsedImplicitly]
		public void onPolylineClick(AndroidJavaObject polylineAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onPolylineClick(new Polyline(polylineAJO)));
		}
	}
}