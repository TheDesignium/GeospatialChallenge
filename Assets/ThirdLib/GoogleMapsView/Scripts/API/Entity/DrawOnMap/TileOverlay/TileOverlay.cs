using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	[SuppressMessage("ReSharper", "RedundantNameQualifier")]
	public class TileOverlay
	{
		bool _wasRemoved;

		readonly AndroidJavaObject _ajo;

#pragma warning disable 0414
		readonly IntPtr _overlayPtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public TileOverlay(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		public TileOverlay()
		{
		}

		public TileOverlay(IntPtr overlayPtr, IntPtr mapPtr)
		{
			_overlayPtr = overlayPtr;
			_mapPtr = mapPtr;
		}

		/// <summary>
		/// Gets this tile overlay's id. The id will be unique amongst all TileOverlays on a map.
		/// </summary>
		/// <value>The tile overlay identifier.</value>
		[PublicAPI]
		public string Id
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return string.Empty;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewTileOverlayGetId(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getId");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Gets or sets the index of the Z. Overlays with higher zIndices are drawn above those with lower indices.
		/// </summary>
		/// <value>The index of the Z.</value>
		[PublicAPI]
		public float ZIndex
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewTileOverlayGetZIndex(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getZIndex");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewTileOverlaySetZIndex(_overlayPtr, (int) value);
#endif

				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Gets/sets the transparency of this tile overlay.
		/// Transparency of the tile overlay in the range [0..1] where 0 means the overlay is opaque and 1 means the overlay is fully transparent.
		/// </summary>
		/// <value>The transparency of the marker.</value>
		[PublicAPI]
		public float Transparency
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewTileOverlayGetTransparency(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getTransparency");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewTileOverlaySetTransparency(_overlayPtr, value);
#endif

				SetValueIfAndroid("setTransparency", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this tile overlay is visible.
		/// If this tile overlay is not visible then it will not be drawn. All other state is preserved. Tile overlays are visible by default.
		/// </summary>
		/// <value><c>true</c> if this tile overlay is visible; otherwise, <c>false</c>.</value>
		[PublicAPI]
		public bool IsVisible
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewTileOverlayIsVisible(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isVisible");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewTileOverlaySetVisible(_overlayPtr, _mapPtr, value);
#endif

				SetValueIfAndroid("setVisible", value);
			}
		}

		/// <summary>
		/// Gets/sets whether the overlay tiles should fade in.
		/// </summary>
		[PublicAPI]
		public bool FadeIn
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewTileOverlayIsVisible(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("getFadeIn");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewTileOverlaySetVisible(_overlayPtr, _mapPtr, value);
#endif

				SetValueIfAndroid("setFadeIn", value);
			}
		}

		/// <summary>
		/// Clears the tile cache so that all tiles will be requested again from the <see cref="TileProvider"/>
		/// </summary>
		[PublicAPI]
		public void ClearTileCache()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewTileOverlayClearTileCache(_overlayPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("clearTileCache");
			}
		}

		/// <summary>
		/// Removes this tile overlay from the map.
		/// </summary>
		[PublicAPI]
		public void Remove()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewTileOverlayRemove(_overlayPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("remove");
			}

			_wasRemoved = true;
		}

		public override string ToString()
		{
			return $"[TileOverlay: Id={Id}, ZIndex={ZIndex}, Transparency={Transparency}, IsVisible={IsVisible}, FadeIn={FadeIn}]";
		}

		void CheckIfRemoved()
		{
			if (_wasRemoved)
			{
				Debug.LogError("This circle was already removed from the map. You can't perform any more operations on it.");
			}
		}

		T GetValueIfAndroid<T>(string methodName)
		{
			return GoogleMapUtils.IsAndroid ? _ajo.MainThreadCall<T>(methodName) : default(T);
		}

		void SetValueIfAndroid(string methodName, params object[] args)
		{
			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCallNonBlocking(methodName, args);
			}
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewTileOverlayGetId(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern int _googleMapsViewTileOverlayGetZIndex(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewTileOverlaySetZIndex(IntPtr overlayPtr, int zIndex);

		[DllImport("__Internal")]
		static extern float _googleMapsViewTileOverlayGetTransparency(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewTileOverlaySetTransparency(IntPtr overlayPtr, float transparency);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewTileOverlayIsVisible(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewTileOverlaySetVisible(IntPtr overlayPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern void _googleMapsViewTileOverlayRemove(IntPtr overlayPtr);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewTileOverlayClearTileCache(IntPtr overlayPtr);
#endif
	}
}