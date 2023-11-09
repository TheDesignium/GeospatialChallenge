using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using NinevaStudios.GoogleMaps.MiniJSON;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Class to create and manage clusters of markers
	/// </summary>
	[PublicAPI]
	public sealed class ClusterManager
	{
		const string ClusterRendererClass = "com.deadmosquitogames.gmaps.clustering.CustomClusterRenderer";

		const string ClusterManagerClass = "com.google.maps.android.clustering.ClusterManager";

		readonly GoogleMapsView _googleMapsView;

		AndroidJavaObject _clusterManagerAJO;
#pragma warning disable 0414
		IntPtr _clusterManagerPtr = IntPtr.Zero;
#pragma warning restore 0414

		/// <summary>
		/// Creates new cluster manager for existing map view.
		/// </summary>
		/// <param name="googleMapsView">Map view</param>
		/// <exception cref="ArgumentNullException">Thrown if map view is null</exception>
		[PublicAPI]
		public ClusterManager([NotNull] GoogleMapsView googleMapsView)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (googleMapsView == null)
			{
				throw new ArgumentNullException(nameof(googleMapsView));
			}

			_googleMapsView = googleMapsView;

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO = new AndroidJavaObject(ClusterManagerClass, JniToolkitUtils.Activity, _googleMapsView.GoogleMapAJO);
					var renderer = new AndroidJavaObject(ClusterRendererClass, JniToolkitUtils.Activity, _googleMapsView.GoogleMapAJO, _clusterManagerAJO);
					_clusterManagerAJO.Call("setRenderer", renderer);
					_googleMapsView.AJO.Call("setClusterOnCameraIdleListener", _clusterManagerAJO);
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_clusterManagerPtr = _createGoogleMapsViewClusterManager(googleMapsView.MapPtr);
#endif
		}

		/// <summary>
		/// Add item to the cluster manager.
		/// </summary>
		/// <param name="clusterItem">Item to add to the cluster. Must not be null.</param>
		/// <exception cref="ArgumentNullException">Thrown if item is null.</exception>
		[PublicAPI]
		public void AddItem([NotNull] ClusterItem clusterItem)
		{
			if (clusterItem == null)
			{
				throw new ArgumentNullException(nameof(clusterItem));
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO.Call("addItem", clusterItem.ToAJO());
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewClusterManagerAddSingleItem(_clusterManagerPtr, Json.Serialize(clusterItem.ToDictionary()));
#endif
		}

		/// <summary>
		/// Add a list of cluster items to the cluster
		/// </summary>
		/// <param name="clusterItems">Items to add. Must not be null or empty.</param>
		/// <exception cref="ArgumentNullException">Thrown if collection is null</exception>
		/// <exception cref="ArgumentException">Thrown if collection is empty</exception>
		[PublicAPI]
		public void AddItems([NotNull] List<ClusterItem> clusterItems)
		{
			if (clusterItems == null)
			{
				throw new ArgumentNullException(nameof(clusterItems));
			}

			if (clusterItems.Count == 0)
			{
				throw new ArgumentException("Value cannot be an empty collection.", nameof(clusterItems));
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				var serialized = SerializeClusterItems(clusterItems);
				JniToolkitUtils.RunOnUiThread(() =>
				{
					using (var managerClass = new AndroidJavaClass(GoogleMapsView.ClassName))
					{
						managerClass.CallStatic("addClusterItems", _clusterManagerAJO, serialized, JniToolkitUtils.Activity);
					}
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var itemsToSerialize = clusterItems.ConvertAll(item => item.ToDictionary());
			_googleMapsViewClusterManagerAddItems(_clusterManagerPtr, Json.Serialize(itemsToSerialize));
#endif
		}

		static string SerializeClusterItems(List<ClusterItem> clusterItems)
		{
			return Json.Serialize(clusterItems.Select(clusterItem => clusterItem.ToDictionary()).Cast<object>().ToList());
		}

		/// <summary>
		/// Remove all the items from the cluster.
		/// </summary>
		[PublicAPI]
		public void ClearItems()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO.Call("clearItems");
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewClusterManagerClearItems(_clusterManagerPtr);
#endif
		}

		void AndroidCluster()
		{
			_clusterManagerAJO.Call("cluster");
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern IntPtr _createGoogleMapsViewClusterManager(IntPtr mapPtr);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerAddSingleItem(IntPtr managerPtr, string item);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerAddItems(IntPtr managerPtr, string items);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerClearItems(IntPtr managerPtr);
#endif
	}
}