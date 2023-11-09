using System;
using NinevaStudios.GoogleMaps.MiniJSON;
using System.Collections.Generic;
using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DemoUtils
{
	public static readonly LatLng BerlinLatLng = new LatLng(BerlinLatitude, BerlinLongitude);
	public static readonly LatLng MelbourneLatLng = new LatLng(-37.8715947, 144.8284128);

	public static readonly LatLngBounds BerlinLatLngBounds =
		new LatLngBounds(BerlinLatLng, new LatLng(BerlinLatitude + 10, BerlinLongitude + 10));

	public static readonly List<LatLng> UsaPolylinePoints = new List<LatLng>
	{
		new LatLng(37.002361f, -114.110389f),
		new LatLng(41.958444f, -114.020981f),
		new LatLng(45.008024f, -111.008123f),
		new LatLng(44.987813, -104.078093)
	};

	static readonly List<LatLng> ColoradoHole = new List<LatLng>
	{
		new LatLng(38.37f, -107.32f),
		new LatLng(40.07f, -105.88f),
		new LatLng(38.09f, -103.40)
	};

	public static readonly List<List<LatLng>> ColoradoHoles = new List<List<LatLng>> {ColoradoHole};

	public static readonly List<LatLng> ColoradoBorders = new List<LatLng>
	{
		new LatLng(37f, -109.04),
		new LatLng(41.05f, -109.04),
		new LatLng(41.05f, -102.04f),
		new LatLng(37f, -102.04)
	};

	const float BerlinLatitude = 52.4965725F;
	const float BerlinLongitude = 13.3933283F;

	static readonly Color[] ALT_HEATMAP_GRADIENT_COLORS =
	{
		new Color32(255, 0, 255, 0), // transparent
		new Color32(255, 0, 255, 255 / 3 * 2),
		new Color32(0, 0, 191, 255),
		new Color32(0, 0, 0, 127),
		new Color32(0, 255, 0, 0)
	};

	static readonly float[] ALT_HEATMAP_GRADIENT_START_POINTS =
	{
		0.0f, 0.10f, 0.20f, 0.60f, 1.0f
	};

	public static CircleOptions CreateInitialCircleOptions()
	{
		const float sydneyLatitude = -34;
		const float sydneyLongitude = 151;

		// on iOS width is in iOS points, and pixels on Android
		var width = GoogleMapUtils.IsAndroid ? 20 : 2;

		// Create a circle in Sydney, Australia
		return new CircleOptions()
			.Center(new LatLng(sydneyLatitude, sydneyLongitude))
			.Radius(100000f)
			.StrokeWidth(width)
			.StrokeColor(Color.red)
			.FillColor(new Color(0, 1, 0, 0.5f))
			.Visible(true)
			.Clickable(true)
			.ZIndex(1);
	}

	public static MarkerOptions CreateInitialMarkerOptions()
	{
		// Create a marker in London, Great Britain
		var london = new LatLng(51.5285582f, -0.2417005f);

		return NewMarkerOptions(london, NewCustomDescriptor());
	}

	static ImageDescriptor NewCustomDescriptor()
	{
		return ImageDescriptor.FromAsset("map-marker-icon.png");
	}

	public static MarkerOptions CreateTexture2DMarkerOptions(Texture2D tex)
	{
		var london = new LatLng(BerlinLatitude, BerlinLongitude);

		return NewMarkerOptions(london, ImageDescriptor.FromTexture2D(tex));
	}

	static MarkerOptions NewMarkerOptions(LatLng position, ImageDescriptor imageDescriptor)
	{
		return new MarkerOptions()
			.Position(position)
			.Icon(imageDescriptor) // image must be in StreamingAssets folder!
			.Alpha(0.8f) // make semi-transparent image
			.Anchor(0.5f, 1f) // anchor point of the image
			.InfoWindowAnchor(0.5f, 1f)
			.Draggable(true)
			.Flat(false)
			.Rotation(30f) // Rotate marker a bit
			.Snippet("Snippet Text")
			.Title("Title Text")
			.Visible(true)
			.ZIndex(1f);
	}

	public static MarkerOptions CreateTintedMarkerOptions()
	{
		return new MarkerOptions()
			.Icon(ImageDescriptor.DefaultMarker(ImageDescriptor.HUE_AZURE));
	}

	public static GroundOverlayOptions CreateInitialGroundOverlayOptions(ImageDescriptor image)
	{
		return new GroundOverlayOptions()
//                .Position(new LatLng(BerlinLatitude, BerlinLongitude), 303000, 150000)
			.PositionFromBounds(BerlinLatLngBounds)
			.Image(image) // image must be in StreamingAssets folder!
			.Anchor(1, 1)
			.Bearing(45)
			.Clickable(true)
			.Transparency(0)
			.Visible(true)
			.ZIndex(1);
	}

	public static GroundOverlayOptions CreateNewArkGroundOverlay(Texture2D image)
	{
		var southwest = new LatLng(40.712216, -74.22655);
		var northeast = new LatLng(40.773941, -74.12544);
		var latLngBounds = new LatLngBounds(southwest, northeast);
		return new GroundOverlayOptions()
			.PositionFromBounds(latLngBounds)
			.Transparency(0.3f)
			.Image(ImageDescriptor.FromTexture2D(image));
	}

	public static GroundOverlayOptions CreateTalkeetnaGroundOverlayForIos()
	{
		var latLng = new LatLng(62.341145, -150.14637);
		return new GroundOverlayOptions()
			// This method with position and zoomLevel is only available on iOS
			.PositionForIos(latLng, 12.2f)
			.Transparency(0.3f)
			.Image(ImageDescriptor.FromAsset("talkeetna.png"));
	}

	public static MarkerOptions RandomColorMarkerOptions(LatLng point)
	{
		return new MarkerOptions()
			.Position(point)
			.Icon(ImageDescriptor.DefaultMarker(Random.Range(0, 360)));
	}

	public static CircleOptions RandomColorCircleOptions(LatLng point)
	{
		return new CircleOptions()
			.Center(point)
			.Radius(1000000)
			.FillColor(ColorUtils.RandomColor())
			.StrokeColor(ColorUtils.RandomColor());
	}

	public static PolylineOptions CreateInitialPolylineOptions()
	{
		// on iOS width is in iOS points, and pixels on Android
		var width = GoogleMapUtils.IsAndroid ? 20 : 2;

		return new PolylineOptions()
			.Add(new LatLng(10, 10), new LatLng(30, 30), new LatLng(-30, 30), new LatLng(50, 50))
			.Clickable(true)
			.Color(Color.red)
			.StartCap(new CustomCap(ImageDescriptor.FromAsset("cap.png"), 16f))
			.EndCap(new RoundCap())
			.JointType(JointType.Round)
			.Geodesic(false)
			.Visible(true)
			.Width(width)
			.ZIndex(1f);
	}

	public static PolygonOptions CreateColoradoStatePolygonOptions()
	{
		// on iOS width is in iOS points, and pixels on Android
		var width = GoogleMapUtils.IsAndroid ? 20 : 2;

		return new PolygonOptions()
			.Add(ColoradoBorders)
			.FillColor(new Color(0.5f, 0.5f, 0.5f, 0.2f))
			.StrokeColor(new Color(0.5f, 0f, 0f, 0.8f))
			.StrokeWidth(width)
			.StrokeJointType(JointType.Round)
			.AddHole(ColoradoHole)
			.Visible(true)
			.Clickable(true)
			.Geodesic(false)
			.ZIndex(1f);
	}

	public static LatLng RandomLatLng()
	{
		return new LatLng(Random.Range(-90f, 90f), Random.Range(-180f, 180f));
	}

	public static List<LatLng> RandomLocations(int size)
	{
		var locations = new List<LatLng>(size);
		for (int i = 0; i < size; i++)
		{
			locations.Add(RandomLatLng());
		}

		return locations;
	}

	public static TileOverlayOptions CreateDemoHeatMap(string jsonCoordinates)
	{
		var heatmapData = DeserializeLocations(jsonCoordinates);

		TileProvider tileProvider = new HeatmapTileProvider.Builder()
			.Radius(30)
			.Gradient(new HeatmapGradient(ALT_HEATMAP_GRADIENT_COLORS, ALT_HEATMAP_GRADIENT_START_POINTS, 1000))
			.Data(heatmapData)
			.Build();

		return new TileOverlayOptions()
			.FadeIn(true)
			.Transparency(0.0f)
			.ZIndex(1)
			.Visible(true)
			.TileProvider(tileProvider);
	}

	public static List<LatLng> DeserializeLocations(string coordinatesJson)
	{
		var heatmapData = new List<LatLng>();
		var deserialized = Json.Deserialize(coordinatesJson) as List<object>;
		foreach (var latLng in deserialized)
		{
			var latLngDic = latLng as Dictionary<string, object>;
			var item = new LatLng(latLngDic.GetDouble("lat"), latLngDic.GetDouble("lng"));
			heatmapData.Add(item);
		}

		return heatmapData;
	}

	public static List<ClusterItem> DeserializeClusterMarkers(string jsonData, Texture2D texture)
	{
		var clusterItems = new List<ClusterItem>();
		var deserialized = Json.Deserialize(jsonData) as List<object>;
		for (var i = 0; i < deserialized.Count; i++)
		{
			var item = deserialized[i];
			var itemDic = item as Dictionary<string, object>;
			var latLng = new LatLng(itemDic.GetDouble("lat"), itemDic.GetDouble("lng"));
			var title = itemDic.GetStr("title") ?? Guid.NewGuid().ToString();
			var snippet = itemDic.GetStr("snippet") ?? Guid.NewGuid().ToString();
			var markerOptions = RandomColorMarkerOptions(latLng)
				.Icon(GetDescriptor(i, texture))
				.Rotation(Random.Range(-45f, 45f))
				.Title(title)
				.Snippet(snippet);
			clusterItems.Add(new ClusterItem(markerOptions));
		}

		return clusterItems;
	}

	static ImageDescriptor GetDescriptor(int index, Texture2D texture)
	{
		ImageDescriptor descriptor;
		var remainder = index % 4;
		switch (remainder)
		{
			case 0:
				descriptor = ImageDescriptor.DefaultMarker();
				break;
			case 1:
				descriptor = ImageDescriptor.DefaultMarker(Random.Range(0f, 360f));
				break;
			case 2:
				descriptor = ImageDescriptor.FromTexture2D(texture);
				break;
			default:
				descriptor = ImageDescriptor.FromAsset("map-marker-icon.png");
				break;
		}

		return descriptor;
	}
}