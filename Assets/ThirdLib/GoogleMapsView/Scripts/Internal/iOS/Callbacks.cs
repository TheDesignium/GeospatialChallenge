 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
using System.Runtime.InteropServices;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using AOT;

	public static class Callbacks
	{
		internal delegate void OnLocationSelectedDelegate(IntPtr actionPtr, double lat, double lng);

		internal delegate void OnItemClickedDelegate(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr);
		
		internal delegate void ImageResultDelegate(IntPtr callbackPtr, IntPtr byteArrPtr, int arrayLength);
		
		internal delegate void ActionIntCallbackDelegate(IntPtr actionPtr, int data);
		
		internal delegate void ActionVoidCallbackDelegate(IntPtr actionPtr);

		[MonoPInvokeCallback(typeof(OnLocationSelectedDelegate))]
		public static void OnLocationSelectedCallback(IntPtr actionPtr, double lat, double lng)
		{
			var locationCoordinate2D = new LatLng(lat, lng);
			if (Debug.isDebugBuild)
			{
				Debug.Log("OnLocationSelectedCallback: " + locationCoordinate2D);
			}

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<LatLng>>();
				action(locationCoordinate2D);
			}
		}

		[MonoPInvokeCallback(typeof(OnItemClickedDelegate))]
		public static void OnMarkerClickedCallback(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<Marker>>();
				var marker = new Marker(itemPtr, mapPtr);
				action(marker);
			}
		}
		
		[MonoPInvokeCallback(typeof(OnItemClickedDelegate))]
		public static void OnCircleClickedCallback(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<Circle>>();
				var circle = new Circle(itemPtr, mapPtr);
				action(circle);
			}
		}
		
		[MonoPInvokeCallback(typeof(OnItemClickedDelegate))]
		public static void OnGroundOverlayClickedCallback(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<GroundOverlay>>();
				var groundOverlay = new GroundOverlay(itemPtr, mapPtr);
				action(groundOverlay);
			}
		}
		
		[MonoPInvokeCallback(typeof(OnItemClickedDelegate))]
		public static void OnPolylineClickedCallback(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<Polyline>>();
				var polyline = new Polyline(itemPtr, mapPtr);
				action(polyline);
			}
		}
		
		[MonoPInvokeCallback(typeof(OnItemClickedDelegate))]
		public static void OnPolygonClickedCallback(IntPtr actionPtr, IntPtr itemPtr, IntPtr mapPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<Polygon>>();
				var polygon = new Polygon(itemPtr, mapPtr);
				action(polygon);
			}
		}
		
		[MonoPInvokeCallback(typeof(ActionIntCallbackDelegate))]
		public static void ActionIntCallback(IntPtr actionPtr, int data)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ActionIntCallback: " + data);
			}
			
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<int>>();
				action(data);
			}
		}

		[MonoPInvokeCallback(typeof(ActionVoidCallbackDelegate))]
		public static void ActionVoidCallback(IntPtr actionPtr)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ActionVoidCallback");
			}
			
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action>();
				action();
			}
		}
		
		[MonoPInvokeCallback(typeof(ImageResultDelegate))]
		public static void ImageResultCallback(IntPtr callbackPtr, IntPtr byteArrPtr, int arrayLength)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Picked img ptr: " + byteArrPtr.ToInt32() + ", array length: " + arrayLength);
			}

			var buffer = new byte[arrayLength];

			Marshal.Copy(byteArrPtr, buffer, 0, arrayLength);
			var tex = new Texture2D(2, 2);
			tex.LoadImage(buffer);

			if (callbackPtr != IntPtr.Zero)
			{
				var action = callbackPtr.Cast<Action<Texture2D>>();
				action(tex);
			}
		}
	}
}
#endif