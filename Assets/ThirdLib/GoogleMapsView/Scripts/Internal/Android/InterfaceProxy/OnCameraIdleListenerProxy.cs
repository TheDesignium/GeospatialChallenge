// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnCameraIdleListenerProxy : AndroidJavaProxy
	{
		readonly Action _onCameraIdle;

		public OnCameraIdleListenerProxy(Action onCameraIdle)
			: base("com.google.android.gms.maps.GoogleMap$OnCameraIdleListener")
		{
			_onCameraIdle = onCameraIdle;
		}

		[UsedImplicitly]
		public void onCameraIdle()
		{
			GoogleMapsSceneHelper.Queue(() => _onCameraIdle());
		}
	}
}