// ReSharper disable InconsistentNaming

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnCircleClickListenerProxy : AndroidJavaProxy
	{
		readonly Action<Circle> _onCircleClick;

		public OnCircleClickListenerProxy(Action<Circle> onCircleClick)
			: base("com.google.android.gms.maps.GoogleMap$OnCircleClickListener")
		{
			_onCircleClick = onCircleClick;
		}

		[UsedImplicitly]
		public void onCircleClick(AndroidJavaObject circleAJO)
		{
			GoogleMapsSceneHelper.Queue(() => _onCircleClick(new Circle(circleAJO)));
		}
	}
}