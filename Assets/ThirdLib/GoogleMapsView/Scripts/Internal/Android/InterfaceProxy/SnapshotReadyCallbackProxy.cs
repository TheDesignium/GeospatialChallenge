using System;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	public class SnapshotReadyCallbackProxy : AndroidJavaProxy
	{
		readonly Action<Texture2D> _onSnapshotReady;

		public SnapshotReadyCallbackProxy(Action<Texture2D> onSnapshotReady) : base(
			"com.google.android.gms.maps.GoogleMap$SnapshotReadyCallback")
		{
			_onSnapshotReady = onSnapshotReady;
		}

		[UsedImplicitly]
		public void onSnapshotReady(AndroidJavaObject bitmapAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onSnapshotReady(BitmapUtils.Texture2DFromBitmap(bitmapAJO)));
		}
	}
}