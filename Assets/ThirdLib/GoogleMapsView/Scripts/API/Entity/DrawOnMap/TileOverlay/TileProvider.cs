using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
    [PublicAPI]
    public interface TileProvider
    {
        AndroidJavaObject AJO { get; }
        
        Dictionary<string, object> Dictionary { get; }
    }
}