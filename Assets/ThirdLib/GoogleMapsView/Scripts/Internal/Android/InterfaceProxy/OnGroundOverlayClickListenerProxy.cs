// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnGroundOverlayClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<GroundOverlay> _onGroundOverlayClick;

		public OnGroundOverlayClickListenerProxy(Action<GroundOverlay> onGroundOverlayClick)
			: base("com.google.android.gms.maps.GoogleMap$OnGroundOverlayClickListener")
		{
			_onGroundOverlayClick = onGroundOverlayClick;
		}

		[UsedImplicitly]
		public void onGroundOverlayClick(AndroidJavaObject ajo)
		{
			GoogleMapsSceneHelper.Queue(() => _onGroundOverlayClick(new GroundOverlay(ajo)));
		}
	}
}