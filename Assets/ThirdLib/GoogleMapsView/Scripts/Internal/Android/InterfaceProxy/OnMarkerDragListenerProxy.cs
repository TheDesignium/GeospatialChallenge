using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class OnMarkerDragListenerProxy : AndroidJavaProxy
	{
		readonly Action<Marker> _onMarkerDragStart;
		readonly Action<Marker> _onMarkerDrag;
		readonly Action<Marker> _onMarkerDragEnd;

		public OnMarkerDragListenerProxy(Action<Marker> onMarkerDragStart, Action<Marker> onMarkerDragEnd, Action<Marker> onMarkerDrag)
			: base("com.google.android.gms.maps.GoogleMap$OnMarkerDragListener")
		{
			_onMarkerDragStart = onMarkerDragStart;
			_onMarkerDragEnd = onMarkerDragEnd;
			_onMarkerDrag = onMarkerDrag;
		}
		
		[UsedImplicitly]
		public void onMarkerDragStart(AndroidJavaObject ajo)
		{
			GoogleMapsSceneHelper.Queue(() => _onMarkerDragStart(new Marker(ajo)));
		}
		
		[UsedImplicitly]
		public void onMarkerDragEnd(AndroidJavaObject ajo)
		{
			GoogleMapsSceneHelper.Queue(() => _onMarkerDragEnd(new Marker(ajo)));
		}

		[UsedImplicitly]
		public void onMarkerDrag(AndroidJavaObject ajo)
		{
			GoogleMapsSceneHelper.Queue(() => _onMarkerDrag(new Marker(ajo)));
		}
	}
}