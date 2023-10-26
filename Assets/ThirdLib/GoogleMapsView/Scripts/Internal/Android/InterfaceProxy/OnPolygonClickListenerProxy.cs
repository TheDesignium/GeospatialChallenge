// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnPolygonClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<Polygon> _onPolygonClick;

		public OnPolygonClickListenerProxy(Action<Polygon> onPolygonClick)
			: base("com.google.android.gms.maps.GoogleMap$OnPolygonClickListener")
		{
			_onPolygonClick = onPolygonClick;
		}

		[UsedImplicitly]
		public void onPolygonClick(AndroidJavaObject polygonAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onPolygonClick(new Polygon(polygonAJO)));
		}
	}
}