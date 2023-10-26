using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
    /// <summary>
    /// Defines options for a <see cref="TileOverlay"/>.
    /// </summary>
    [PublicAPI]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    public class TileOverlayOptions
    {
        const string TileOverlayOptionsClass = "com.google.android.gms.maps.model.TileOverlayOptions";

        const string ZIndexMethodName = "zIndex";
        const string IsVisibleMethodName = "visible";
        const string TransparencyMethodName = "transparency";
        const string TileProviderMethodName = "tileProvider";
        const string FadeInMethodName = "fadeIn";

        TileProvider _tileProvider = null;
        float _transparency = 0.0f;
        bool _visible = true;
        float _zIndex = 0f;
        bool _fadeIn = true;

        public bool HasTileProvider => _tileProvider != null;

        public AndroidJavaObject ToAJO()
        {
            if (GoogleMapUtils.IsNotAndroid)
            {
                return null;
            }

            var ajo = new AndroidJavaObject(TileOverlayOptionsClass);

            ajo.CallAJO(TileProviderMethodName, _tileProvider.AJO);
            ajo.CallAJO(ZIndexMethodName, _zIndex);
            ajo.CallAJO(IsVisibleMethodName, _visible);
            ajo.CallAJO(TransparencyMethodName, _transparency);
            ajo.CallAJO(FadeInMethodName, _fadeIn);

            return ajo;
        }
        
        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>
            {
	            [TileProviderMethodName] = _tileProvider.Dictionary,
	            [ZIndexMethodName] = _zIndex,
	            [IsVisibleMethodName] = _visible,
	            [TransparencyMethodName] = _transparency,
	            [FadeInMethodName] = _fadeIn
            };

            return result;
        }

        /// <summary>
        /// Specifies whether the tiles should fade in. The default is <code>true</code>.
        /// </summary>
        [PublicAPI]
        public TileOverlayOptions FadeIn(bool fadeIn)
        {
            _fadeIn = fadeIn;
            return this;
        }

        /// <summary>
        /// Specifies the tile provider to use for this tile overlay.
        /// </summary>
        [PublicAPI]
        public TileOverlayOptions TileProvider(TileProvider tileProvider)
        {
            _tileProvider = tileProvider;
            return this;
        }

        /// <summary>
        /// Specifies the transparency of the tile overlay. The default transparency is 0 (opaque).
        /// </summary>
        [PublicAPI]
        public TileOverlayOptions Transparency(float transparency)
        {
            _transparency = transparency;
            return this;
        }

        /// <summary>
        /// Specifies the visibility for the tile overlay.
        /// </summary>
        [PublicAPI]
        public TileOverlayOptions Visible(bool visible)
        {
            _visible = visible;
            return this;
        }

        /// <summary>
        /// Specifies the tile overlay's zIndex, i.e., the order in which it will be drawn where overlays with larger values are drawn above those with lower values.
        /// </summary>
        [PublicAPI]
        public TileOverlayOptions ZIndex(float zIndex)
        {
            _zIndex = zIndex;
            return this;
        }
    }
}