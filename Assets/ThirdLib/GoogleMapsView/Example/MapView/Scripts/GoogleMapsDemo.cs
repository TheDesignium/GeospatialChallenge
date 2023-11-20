#if UNITY_2018_3_OR_NEWER && PLATFORM_ANDROID
	using UnityEngine.Android;
#endif

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;

public class GoogleMapsDemo : MonoBehaviour
{

	public directionsPOI poi;

	public bool directions;
	public GameObject frame;
	public Color panelColour;

#pragma warning disable 0649

	#region map_options

	public Texture2D newArkAreaImage;
	public Texture2D icon;

	public Texture2D[] icons;

	public TextAsset customStyleJson;
	public TextAsset heatmapDataJsonPoliceStations;
	public TextAsset heatmapDataJsonMedicare;
	public TextAsset markerClusterData;

	[Header("Bounds")] public Toggle boundsToggle;

	[Header("Options Toggles")] public Toggle ambientToggle;

	public Toggle compassToggle;
	public Toggle liteModeToggle;
	public Toggle mapToolbarToggle;
	public Toggle rotateGesturesToggle;
	public Toggle scrollGesturesToggle;
	public Toggle tiltGesturesToggle;
	public Toggle zoomGesturesToggle;
	public Toggle zoomControlsToggle;
	public Toggle trafficControlsToggle;

	[Header("Map Type")] public Dropdown mapType;

	[Header("Min/Max Zoom")] public InputField minZoom;

	public InputField maxZoom;

	[Header("Camera Position")] public Slider camPosLat;

	public Slider camPosLng;
	public Slider camPosZoom;
	public Slider camPosTilt;
	public Slider camPosBearing;

	[Header("Camera Labels")] public Text camPosLatText;

	public Text camPosLngText;
	public Text camPosZoomText;
	public Text camPosTiltText;
	public Text camPosBearingText;

	[Header("Bound South-West")] [Range(-90, 90)] [SerializeField]
	float _boundsSouthWestPosLat;

	[Range(-180, 180)] [SerializeField]
	float _boundsSouthWestPosLng;

	[Header("Bound North-East")] [Range(-90, 90)] [SerializeField]
	float _boundsNorthEastPosLat;

	[Range(-180, 180)] [SerializeField]
	float _boundsNorthEastPosLng;

	#endregion

#pragma warning restore 0649

	#region circle_options

	[Header("Circle Center")] public Slider circleLat;

	public Slider circleLng;
	public Slider circleStokeWidth;
	public Slider circleRadius;
	public Toggle circleVisibilityToggle;
	public Toggle IsCameraMovementAnimatedToggle;

	#endregion

	#region marker_options

	[Header("Marker Center")] public Slider markerLat;

	public Slider markerLng;

	public Text isInfoWindowEnabledText;

	#endregion

	#region snapshot

	[Header("Map snapshot")] public Image snapshotImage;
	public Image frameImage;
	#endregion

	public RectTransform rect;

	public GoogleMapsView _map;
	Circle _circle;
	Marker _marker;
	GroundOverlay _groundOverlay;
	Polyline _polyline;
	Polygon _coloradoPolygon;
	TileOverlay _heatmap;
	ClusterManager _clusterManager;

	public bool movingMap;
	public bool idleMap;

	void Awake()
	{
		SetupEvents();
		SetInitialOptionsValues();
	}

	void Start()
	{
		// Show the map when the demo starts
		OnShow();
	}

	void SetInitialOptionsValues()
	{
		mapType.value = (int)GoogleMapType.Normal;

		// Camera position
		//camPosLat.value = 52.0779648f;
		//camPosLng.value = 4.334087f;
		//camPosZoom.value = 2f;
		camPosTilt.value = 1f;
		camPosBearing.value = 0f;

		// Zoom constraints
		minZoom.text = "1.0";
		maxZoom.text = "20.0";
	}

	public GroundOverlay previousOverlay;
	public Polyline previousLine;

	public void setArea()
	{
		LatLng loc = new LatLng(35.6255625f,139.71943749999997f);

		LatLng A = new LatLng(35.625625f,139.71949999999998f); //northeast
		LatLng B = new LatLng(35.6255f,139.71949999999998f); //southeast - modified
		LatLng C = new LatLng(35.6255f,139.71937499999996f); //southwest
		LatLng D = new LatLng(35.625625f,139.71937499999996f); //northwest - modified

		var polyline = _map.AddPolyline(CreateInitialPolylineOptions(A,B,C,D));

		const int zoom = 18;
		AnimateCamera(CameraUpdate.NewLatLngZoom(loc, zoom));
	}


	GroundOverlayOptions CreateInitialGroundOverlayOptions(float loclatf, float loclonf, float ewDistance, float nsDistance)
	{
		return new GroundOverlayOptions()
				.Position(new LatLng(loclatf, loclonf), ewDistance, nsDistance)
	//		.PositionFromBounds(BerlinLatLngBounds)
			.Image(ImageDescriptor.FromAsset("overlay.png")) // image must be in StreamingAssets folder!
			.Anchor(0.5f, 0.5f)
			.Bearing(0)
			.Clickable(true)
			.Transparency(0.3f)
			.Visible(true)
			.ZIndex(1);
	}

	public void setAreaPlusCode(string ne, string sw)
	{
		string[] latLonNE = ne.Split(',');
		string[] latLonSW = sw.Split(',');

		//string ll = latLonNE[1] + "," + latLonNE[0];

		float nelatf = 0;
		float nelonf = 0;
		float swlatf = 0;
		float swlonf = 0;

		float.TryParse(latLonNE[1], out nelatf);
		float.TryParse(latLonNE[0], out nelonf);
		float.TryParse(latLonSW[1], out swlatf);
		float.TryParse(latLonSW[0], out swlonf);

		LatLng A = new LatLng(nelatf,nelonf); //northeast
		LatLng B = new LatLng(swlatf,nelonf); //southeast - modified
		LatLng C = new LatLng(swlatf,swlonf); //southwest
		LatLng D = new LatLng(nelatf,swlonf); //northwest - modified

		var polyline = _map.AddPolyline(CreateInitialPolylineOptions(A,B,C,D));
	}

	public void setPoint()
	{
		addMapPin(35.625579f,139.719445f);
	}

	public static PolylineOptions CreateInitialPolylineOptions(LatLng A, LatLng B, LatLng C, LatLng D)
	{
	    return new PolylineOptions()
	        .Add(A, B, C, D, A)
	        .Clickable(false)
	        .Color(Color.red)
	        .StartCap(new CustomCap(ImageDescriptor.FromAsset("cap.png"), 16f))
	        .EndCap(new RoundCap())
	        .JointType(JointType.Round)
	        .Geodesic(false)
	        .Visible(true)
	        .Width(4)
	        .ZIndex(1f);
	}

	public void setRoute(List<LatLng> lllist)
	{
		var polyline = _map.AddPolyline(CreateRoutePolylineOptions(lllist));
		const int zoom = 14;
		AnimateCamera(CameraUpdate.NewLatLngZoom(lllist[lllist.Count()/2], zoom));
		addMapPin((float)lllist[lllist.Count() - 1].Latitude, (float)lllist[lllist.Count() - 1].Longitude);
		//_map.fit
	}

	List<string> tempLatLon = new List<string>();

	public void setRouteStrings(List<string> latlngList)
	{
		float templat = 0;
		float templon = 0;
		List<LatLng> lllist = new List<LatLng>();
		foreach(string s in latlngList)
		{
			tempLatLon.Clear();
			tempLatLon = s.Split(",").ToList();
			float.TryParse(tempLatLon[0], out templat);
			float.TryParse(tempLatLon[1], out templon);
			LatLng templl = new LatLng(templat,templon);
			lllist.Add(templl);
		}

		var polyline = _map.AddPolyline(CreateRoutePolylineOptions(lllist));
	}

	public void setZoomStrings(string zoomlatlng)
	{
		List<string> tempzoomLatLon = new List<string>();
		float templat = 0;
		float templon = 0;
		tempzoomLatLon.Clear();
		tempzoomLatLon = zoomlatlng.Split(",").ToList();
		float.TryParse(tempzoomLatLon[0], out templat);
		float.TryParse(tempzoomLatLon[1], out templon);
		LatLng templl = new LatLng(templat,templon);
		const int zoom = 16;
		AnimateCamera(CameraUpdate.NewLatLngZoom(templl, zoom));
	}

	public static PolylineOptions CreateRoutePolylineOptions(List<LatLng> ll)
	{
			return new PolylineOptions()
					.Add(ll)
					.Clickable(false)
					.Color(Color.blue)
					.StartCap(new CustomCap(ImageDescriptor.FromAsset("cap.png"), 16f))
					.EndCap(new RoundCap())
					.JointType(JointType.Round)
					.Geodesic(false)
					.Visible(true)
					.Width(5)
					.ZIndex(1f);
	}

	public void setbyGPS(float latf, float lonf)
    {
			camPosLat.value = latf;
			camPosLng.value = lonf;
			camPosZoom.value = 15f;
		}

	public void addMapPin(float latf, float lonf)
    {
		LatLng point = new LatLng(latf, lonf);
		_map.AddMarker(DemoUtils.RandomColorMarkerOptions(point));
	}

	public void addMapPinLatLng(LatLng point)
  {
		_map.AddMarker(DemoUtils.RandomColorMarkerOptions(point));
	}

	void SetupEvents()
	{
		// Camera position
		camPosLat.onValueChanged.AddListener(newValue => { camPosLatText.text = $"Lat:{newValue}"; });
		camPosLng.onValueChanged.AddListener(newValue => { camPosLngText.text = $"Lng:{newValue}"; });
		camPosZoom.onValueChanged.AddListener(
			newValue => { camPosZoomText.text = $"Zoom:{newValue}"; });
		camPosTilt.onValueChanged.AddListener(
			newValue => { camPosTiltText.text = $"Tilt:{newValue}"; });
		camPosBearing.onValueChanged.AddListener(newValue => { camPosBearingText.text = $"Bearing:{newValue}"; });
	}

	/// <summary>
	/// Shows the <see cref="GoogleMapsView"/>
	/// </summary>
	[UsedImplicitly]
	public void OnShow()
	{
		Dismiss();

		GoogleMapsView.CreateAndShow(CreateMapViewOptions(), RectTransformToScreenSpace(rect), OnMapReady);
	}

	void OnMapReady(GoogleMapsView googleMapsView)
	{
		_map = googleMapsView;
		_map.SetPadding(0, 0, 0, 0);

		var isStyleUpdateSuccess = _map.SetMapStyle(customStyleJson.text);
		if (isStyleUpdateSuccess)
		{
			Debug.Log("Successfully updated style of the map");
		}
		else
		{
			Debug.LogError("Setting new map style failed.");
		}

#if UNITY_2018_3_OR_NEWER && PLATFORM_ANDROID
			if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
			{
				Permission.RequestUserPermission(Permission.FineLocation);
			}
#endif

		// UNCOMMENT if testing with showing users location. DON'T FORGET MANIFEST LOCATION PERMISSION!!!
		_map.IsMyLocationEnabled = true;
		_map.IsTrafficEnabled = trafficControlsToggle.isOn;
		_map.UiSettings.IsMyLocationButtonEnabled = true;
		_map.OnOrientationChange += () => { _map.SetRect(RectTransformToScreenSpace(rect)); };

		_map.SetOnCameraMoveStartedListener(moveReason => onTouch("Camera move started because: " + moveReason));
		_map.SetOnCameraIdleListener(() => onTouchIdle("Camera is now idle"));
		_map.SetOnGroundOverlayClickListener(overlay => Debug.Log("Ground overlay clicked: " + overlay));

		_map.SetOnMapClickListener(point =>
		{
			Debug.Log("Map clicked: " + point);

			if(directions == true)
			{
				if(poi != null)
				{
					poi.startOnTap(point.Latitude, point.Longitude);
				}
			}
		});
		_map.SetOnLongMapClickListener(point =>
		{
			Debug.Log("Map long clicked: " + point);
		});

		Debug.Log("Map is ready: " + _map);
	}

	void onTouch(string marker)
	{
		movingMap = true;
		idleMap = false;
	}
	void onTouchIdle(string marker)
	{
		idleMap = true;
	}

	void AddOtherExampleOverlays()
	{
		// New Ark overlay image
		_map.AddGroundOverlay(DemoUtils.CreateNewArkGroundOverlay(newArkAreaImage));

		// Talkeetna
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_map.AddGroundOverlay(DemoUtils.CreateTalkeetnaGroundOverlayForIos());
		}

		// Medicare
		var medicareHeatmapOptions = DemoUtils.CreateDemoHeatMap(heatmapDataJsonMedicare.text);
		_map.AddTileOverlay(medicareHeatmapOptions);

		// Berlin marker
		_map.AddMarker(DemoUtils.CreateTexture2DMarkerOptions(icon));
	}

	void AddCircle()
	{
		_circle = _map.AddCircle(DemoUtils.CreateInitialCircleOptions());
		CenterCamera(_circle.Center);
	}

	void AddMarker()
	{
		_marker = _map.AddMarker(DemoUtils.CreateInitialMarkerOptions());
		CenterCamera(_marker.Position);
	}

	void AddGroundOverlay()
	{
		var image = ImageDescriptor.FromAsset("overlay.png");
		_groundOverlay = _map.AddGroundOverlay(DemoUtils.CreateInitialGroundOverlayOptions(image));
		CenterCamera(_groundOverlay.Position);
	}

	void AddPolyline()
	{
		_polyline = _map.AddPolyline(DemoUtils.CreateInitialPolylineOptions());
		CenterCamera(new LatLng(10, 10));
	}

	void AddPolygon()
	{
		_coloradoPolygon = _map.AddPolygon(DemoUtils.CreateColoradoStatePolygonOptions());
		CenterCamera(DemoUtils.ColoradoBorders[0]);
	}

	void AddHeatmap()
	{
		var policeStations = DemoUtils.DeserializeLocations(heatmapDataJsonPoliceStations.text);
		_heatmap = _map.AddHeatmapWithDefaultLook(policeStations);
		CenterCamera(DemoUtils.MelbourneLatLng);
	}

	void AddMarkerCluster()
	{
		// Check London to see the cluster
		_clusterManager = new ClusterManager(_map);
		AddClusterItems();
	}

	void AddClusterItems() => _clusterManager?.AddItems(DemoUtils.DeserializeClusterMarkers(markerClusterData.text, icon));

	GoogleMapsOptions CreateMapViewOptions()
	{
		var options = new GoogleMapsOptions();

		options.MapType((GoogleMapType)mapType.value);

		// Camera position
		options.Camera(CameraPosition);

		// Bounds
		if (boundsToggle.isOn)
		{
			options.LatLngBoundsForCameraTarget(Bounds);
		}

		options.AmbientEnabled(ambientToggle.isOn);
		//options.CompassEnabled(compassToggle.isOn);
		options.CompassEnabled(false);
		options.LiteMode(liteModeToggle.isOn);
		//options.MapToolbarEnabled(mapToolbarToggle.isOn);
		options.MapToolbarEnabled(false);
		options.RotateGesturesEnabled(rotateGesturesToggle.isOn);
		options.ScrollGesturesEnabled(scrollGesturesToggle.isOn);
		options.TiltGesturesEnabled(tiltGesturesToggle.isOn);
		options.ZoomGesturesEnabled(zoomGesturesToggle.isOn);
		//options.ZoomControlsEnabled(zoomControlsToggle.isOn);
		options.ZoomControlsEnabled(false);

		options.MinZoomPreference(float.Parse(minZoom.text));
		options.MaxZoomPreference(float.Parse(maxZoom.text));

		return options;
	}

	LatLngBounds Bounds
	{
		get
		{
			var southWest = new LatLng(_boundsSouthWestPosLat, _boundsSouthWestPosLng);
			var northEast = new LatLng(_boundsNorthEastPosLat, _boundsNorthEastPosLng);
			return new LatLngBounds(southWest, northEast);
		}
	}

	CameraPosition CameraPosition => new CameraPosition(
		new LatLng(camPosLat.value, camPosLng.value),
		camPosZoom.value,
		camPosTilt.value,
		camPosBearing.value);

	#region update_buttons_click

	[UsedImplicitly]
	public void OnUpdateCircleButtonClick()
	{
		if (_circle == null)
		{
			AddCircle();
			return;
		}

		Debug.Log("Current circle: " + _circle + ", updating properties...");
		UpdateCircleProperties();
	}

	[UsedImplicitly]
	public void OnRemoveCircleClick()
	{
		if (_circle != null)
		{
			_circle.Remove();
			_circle = null;
			Debug.Log("Circle was removed.");
		}
	}

	[UsedImplicitly]
	public void OnUpdateMarkerButtonClick()
	{
		if (_marker == null)
		{
			AddMarker();
			return;
		}

		Debug.Log("Current marker: " + _marker + ", updating properties...");
		UpdateMarkerProperties();
	}

	[UsedImplicitly]
	public void OnShowMarkerInfoWindow()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.ShowInfoWindow();
		isInfoWindowEnabledText.text = _marker.IsInfoWindowShown.ToString();
	}

	[UsedImplicitly]
	public void OnHideMarkerInfoWindow()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.HideInfoWindow();
		isInfoWindowEnabledText.text = _marker.IsInfoWindowShown.ToString();
	}

	[UsedImplicitly]
	public void OnRemoveMarkerClick()
	{
		if (_marker == null)
		{
			return;
		}

		_marker.Remove();
		_marker = null;
	}

	[UsedImplicitly]
	public void OnUpdateGroundOverlayClick()
	{
		if (_groundOverlay == null)
		{
			AddGroundOverlay();
			return;
		}

		Debug.Log("Current ground overlay: " + _groundOverlay + ", updating properties...");
		UpdateGroundOverlayProperties();
	}

	[UsedImplicitly]
	public void OnRemoveGroundOverlayClick()
	{
		if (_groundOverlay == null)
		{
			return;
		}

		_groundOverlay.Remove();
		_groundOverlay = null;
		Debug.Log("Ground overlay was removed.");
	}

	[UsedImplicitly]
	public void OnUpdateHeatmapOverlayClick()
	{
		if (_heatmap == null)
		{
			AddHeatmap();
			return;
		}

		Debug.Log("Current heatmap overlay: " + _heatmap + ", updating properties...");
		UpdateHeatmapProperties();
	}

	[UsedImplicitly]
	public void OnRemoveHeatmapClick()
	{
		if (_heatmap == null)
		{
			return;
		}

		_heatmap.Remove();
		_heatmap = null;
		Debug.Log("Tile overlay was removed.");
	}

	[UsedImplicitly]
	public void OnUpdatePolylineClick()
	{
		if (_polyline == null)
		{
			AddPolyline();
			return;
		}

		Debug.Log("Current polyline: " + _polyline + ", updating properties...");
		UpdatePolylineProperties();
	}

	[UsedImplicitly]
	public void OnRemovePolylineClick()
	{
		if (_polyline == null)
		{
			return;
		}

		_polyline.Remove();
		_polyline = null;
		Debug.Log("Polyline was removed.");
	}

	[UsedImplicitly]
	public void OnUpdatePolygonClick()
	{
		if (_coloradoPolygon == null)
		{
			AddPolygon();
			return;
		}

		Debug.Log("Current polygon: " + _coloradoPolygon + ", updating properties...");
		UpdatePolygonProperties();
	}

	[UsedImplicitly]
	public void OnRemovePolygonClick()
	{
		if (_coloradoPolygon == null)
		{
			return;
		}

		_coloradoPolygon.Remove();
		_coloradoPolygon = null;
		Debug.Log("Polygon was removed.");
	}

	#region marker_clustering

	[UsedImplicitly]
	public void OnAddClusterItems()
	{
		AddClusterItems();
	}

	[UsedImplicitly]
	public void OnAddSingleClusterItem()
	{
		if (_clusterManager != null)
		{
			var latLng = new LatLng(51.5005642, -0.1241729);
			var clusterItem = new ClusterItem(latLng, "Westminster Bridge", latLng.ToString());
			var westminsterBridge = clusterItem;
			_clusterManager.AddItem(westminsterBridge);
		}
	}

	[UsedImplicitly]
	public void OnClearClusterItems()
	{
		if (_clusterManager != null)
		{
			Debug.Log("Clearing cluster items");
			_clusterManager.ClearItems();
		}
	}

	#endregion

	/// <summary>
	/// Removes all markers, polylines, polygons, overlays, etc from the map.
	/// </summary>
	[UsedImplicitly]
	public void OnClearMapClick()
	{
		if (_map == null)
		{
			return;
		}

		_map.Clear();
		// All the elements are now removed, we cannot access them any more
		_circle = null;
		_marker = null;
		_groundOverlay = null;
		_polyline = null;
		_coloradoPolygon = null;
		_heatmap = null;
	}

	[UsedImplicitly]
	public void OnTestUiSettingsButtonClick(bool enable)
	{
		if (_map == null)
		{
			return;
		}

		EnableAllSettings(_map.UiSettings, enable);
	}

	static void EnableAllSettings(UiSettings settings, bool enable)
	{
		Debug.Log("Current Ui Settings: " + settings);

		// Buttons/other
		settings.IsCompassEnabled = enable;
		settings.IsIndoorLevelPickerEnabled = enable;
		settings.IsMapToolbarEnabled = enable;
		settings.IsMyLocationButtonEnabled = enable;
		settings.IsZoomControlsEnabled = enable;

		// Gestures
		settings.IsRotateGesturesEnabled = enable;
		settings.IsScrollGesturesEnabled = enable;
		settings.IsTiltGesturesEnabled = enable;
		settings.IsZoomGesturesEnabled = enable;
		settings.SetAllGesturesEnabled(enable);
	}

	#endregion

	void UpdateCircleProperties()
	{
		var circleCenter = new LatLng(circleLat.value, circleLng.value);
		CenterCamera(circleCenter);

		_circle.Center = circleCenter;
		_circle.FillColor = ColorUtils.RandomColor();
		_circle.StrokeColor = ColorUtils.RandomColor();
		_circle.StrokeWidth = circleStokeWidth.value;
		_circle.Radius = circleRadius.value;
		_circle.ZIndex = 1f;
		_circle.IsVisible = circleVisibilityToggle.isOn;
		_circle.IsClickable = true;
	}

	void UpdateMarkerProperties()
	{
		var markerPosition = new LatLng(markerLat.value, markerLng.value);
		CenterCamera(markerPosition);

		_marker.Position = markerPosition;
		_marker.Alpha = 1f;
		_marker.IsDraggable = true;
		_marker.Flat = true;
		_marker.IsVisible = true;
		_marker.Rotation = 0;
		_marker.SetAnchor(0.5f, 1f);
		_marker.SetInfoWindowAnchor(0.5f, 1f);
		_marker.Snippet = "Updated Marker";
		_marker.Title = "You can drag this marker";
		_marker.ZIndex = 2;
	}

	void UpdateGroundOverlayProperties()
	{
		CenterCamera(DemoUtils.BerlinLatLng);

		_groundOverlay.Bearing = 135;
		_groundOverlay.IsClickable = true;
		_groundOverlay.IsVisible = true;

		// Mutually exclusive but setting both to test
		_groundOverlay.Position = DemoUtils.BerlinLatLng;
		_groundOverlay.Bounds = DemoUtils.BerlinLatLngBounds;

		_groundOverlay.Transparency = 0.25f;
		_groundOverlay.ZIndex = 3;
		_groundOverlay.SetDimensions(200000); // Just setting twice to test
		_groundOverlay.SetDimensions(200000, 200000);
		_groundOverlay.SetImage(ImageDescriptor.FromAsset("overlay.png"));
		_groundOverlay.SetPositionFromBounds(DemoUtils.BerlinLatLngBounds);
	}

	void UpdateHeatmapProperties()
	{
		CenterCamera(DemoUtils.MelbourneLatLng);

		_heatmap.FadeIn = true;
		_heatmap.ZIndex = 1;
		_heatmap.Transparency = 0.5f;
		_heatmap.IsVisible = true;

		_heatmap.ClearTileCache();
	}

	void UpdatePolylineProperties()
	{
		_polyline.Points = DemoUtils.UsaPolylinePoints;
		_polyline.StartCap = new RoundCap();
		_polyline.StartCap = new SquareCap();
		_polyline.JointType = JointType.Bevel;

		// pixels on Android and points on iOS
		var width = Application.platform == RuntimePlatform.Android ? 25f : 3f;

		_polyline.Width = width;
		_polyline.Color = ColorUtils.RandomColor();
		_polyline.IsGeodesic = false;
		_polyline.IsVisible = true;
		_polyline.IsClickable = true;
		_polyline.ZIndex = 1f;
	}

	void UpdatePolygonProperties()
	{
		_coloradoPolygon.Points = DemoUtils.ColoradoBorders;
		_coloradoPolygon.Holes = new List<List<LatLng>>(); // no holes
		_coloradoPolygon.FillColor = Color.yellow;
		_coloradoPolygon.StrokeColor = Color.blue;
		_coloradoPolygon.StrokeJointType = JointType.Bevel;

		// pixels on Android and points on iOS
		var coloradoPolygonStrokeWidth = Application.platform == RuntimePlatform.Android ? 25f : 3f;

		_coloradoPolygon.StrokeWidth = coloradoPolygonStrokeWidth;
		_coloradoPolygon.IsGeodesic = false;
		_coloradoPolygon.IsVisible = true;
		_coloradoPolygon.IsClickable = true;
		_coloradoPolygon.ZIndex = 1f;
	}

	void Dismiss()
	{
		if (_map != null)
		{
			_map.Dismiss();
			_map = null;
		}
	}

	#region camera_animations

	[UsedImplicitly]
	public void AnimateCameraNewCameraPosition()
	{
		AnimateCamera(CameraUpdate.NewCameraPosition(CameraPosition));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLng()
	{
		AnimateCamera(CameraUpdate.NewLatLng(new LatLng(camPosLat.value, camPosLng.value)));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngBounds1()
	{
		AnimateCamera(CameraUpdate.NewLatLngBounds(Bounds, 10));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngBounds2()
	{
		AnimateCamera(CameraUpdate.NewLatLngBounds(Bounds, 100, 100, 10));
	}

	[UsedImplicitly]
	public void AnimateCameraNewLatLngZoom()
	{
		const int zoom = 10;
		AnimateCamera(CameraUpdate.NewLatLngZoom(new LatLng(camPosLat.value, camPosLng.value), zoom));
	}

	[UsedImplicitly]
	public void AnimateCameraScrollBy()
	{
		const int xPixel = 250;
		const int yPixel = 250;
		AnimateCamera(CameraUpdate.ScrollBy(xPixel, yPixel));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomByWithFixedLocation()
	{
		const int amount = 5;
		const int x = 100;
		const int y = 100;
		AnimateCamera(CameraUpdate.ZoomBy(amount, x, y));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomByAmountOnly()
	{
		const int amount = 5;
		AnimateCamera(CameraUpdate.ZoomBy(amount));
	}

	[UsedImplicitly]
	public void AnimateCameraZoomIn()
	{
		AnimateCamera(CameraUpdate.ZoomIn());
	}

	[UsedImplicitly]
	public void AnimateCameraZoomOut()
	{
		AnimateCamera(CameraUpdate.ZoomOut());
	}

	[UsedImplicitly]
	public void AnimateCameraZoomTo()
	{
		const int zoom = 10;
		AnimateCamera(CameraUpdate.ZoomTo(zoom));
	}

	void AnimateCamera(CameraUpdate cameraUpdate)
	{
		if (_map == null)
		{
			return;
		}

		if (IsCameraMovementAnimatedToggle.isOn)
		{
			_map.AnimateCamera(cameraUpdate);
		}
		else
		{
			_map.MoveCamera(cameraUpdate);
		}
	}

	#endregion

	[UsedImplicitly]
	public void OnLogMyLocation()
	{
		if (_map == null)
		{
			return;
		}

		if (!_map.IsMyLocationEnabled)
		{
			Debug.Log("Location tracking is not enabled. Set 'IsMyLocationButtonEnabled' to 'true' to start tracking location");
			return;
		}

		if (_map.Location != null)
		{
			Debug.Log("My location: " + _map.Location);
		}
		else
		{
			Debug.Log("My location is not available");
		}
	}

	[UsedImplicitly]
	public void OnSetCustomStyle()
	{
		_map?.SetMapStyle(customStyleJson.text);
	}

	[UsedImplicitly]
	public void OnResetToDefaultStyle()
	{
		_map?.SetMapStyle(null);
	}

	[UsedImplicitly]
	public void OnSetMapVisible()
	{
		if (_map != null)
		{
			_map.IsVisible = true;
		}
	}

	[UsedImplicitly]
	public void OnSetMapInvisible()
	{
		if (_map != null)
		{
			_map.IsVisible = false;
		}
	}

	public void toggleMap()
	{
		if(_map.IsVisible == false)
		{
			frame.SetActive(true);
			_map.IsVisible = true;
		}
		else if(_map.IsVisible == true)
		{
			_map.IsVisible = false;
			frame.SetActive(false);
		}
	}

	public void mapOff()
	{
			_map.IsVisible = false;
			frame.SetActive(false);
	}
	public void mapOn()
	{
			_map.IsVisible = true;
			frame.SetActive(true);
	}

	[UsedImplicitly]
	public void OnSetMapPosition() => _map?.SetRect(new Rect(0f, 0f, Screen.width / 2f, Screen.height / 2f));

	[UsedImplicitly]
	public void OnLogMapProperties()
	{
		if (_map != null)
		{
			Debug.Log("Current map: " + _map);
		}
	}

	[UsedImplicitly]
	public void OnUpdateMapProperties()
	{
		if (_map != null)
		{
			_map.MapType = GoogleMapType.Hybrid;
		}
	}

	[UsedImplicitly]
	public void OnTakeSnapshot()
	{
		_map?.TakeSnapshot(texture =>
		{
			Debug.Log("Snapshot captured: " + texture.width + " x " + texture.height);
			snapshotImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		});
	}

	[UsedImplicitly]
	public void LogMapProperties()
	{
		Debug.Log(_map);
	}

	[UsedImplicitly]
	public void LogProjectionData()
	{
		var projection = _map.Projection;
		print(projection.GetVisibleRegion());
		print(projection.FromScreenLocation(new Vector2Int((int)Input.mousePosition.x, (int)Input.mousePosition.y)));
		print(projection.ToScreenLocation(new LatLng(90, 90)));
	}

	#region helpers

	static Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
		rect.x -= transform.pivot.x * size.x;
		rect.y -= (1.0f - transform.pivot.y) * size.y;
		rect.x = Mathf.CeilToInt(rect.x);
		rect.y = Mathf.CeilToInt(rect.y);
		return rect;
	}

	#endregion

	void CenterCamera(LatLng latLng)
	{
		_map?.AnimateCamera(CameraUpdate.NewLatLng(latLng));
	}

	public void setAPI(float _lat, float _lng)
	{
		LatLng point = new LatLng(_lat, _lng);
		_map.AddMarker(DemoUtils.RandomColorMarkerOptions(point));
	}

	public void setLatLng(LatLng point)
	{
		_map.AddMarker(DemoUtils.RandomColorMarkerOptions(point));
	}

	public void setPOI(LatLng point, int icn)
	{
		_map.AddMarker(DemoUtils.CreateTexture2DMarker(point, icons[icn]));
	}

	bool mapout = true;
	public bool fadedIn;

	public void altToggle()
	{
		if(mapout == true)
		{
			screenShot();
			mapout = false;
		}
		else if(mapout == false)
		{
			screenShow();
			mapout = true;
		}
	}

	public void screenShot()
	{
		StartCoroutine(screenGrab());
	}

	IEnumerator screenGrab()
	{
		yield return StartCoroutine(grabTexture());
		yield return new WaitForSeconds(0.4f);
		if(_map != null)
		{
			_map.IsVisible = false;
		}
		//ani.Play("hidemapBottom",0,0);
		StopCoroutine("fadeOutPanel");
		StartCoroutine("fadeOutPanel");
	}

	IEnumerator grabTexture()
	{
		yield return new WaitForEndOfFrame();
		_map?.TakeSnapshot(texture =>
		{
			snapshotImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		});
	}

	public void screenShow()
	{
		StartCoroutine(screenShowNow());
	}

	IEnumerator screenShowNow()
	{
		//ani.Play("showmapBottom",0,0);
		StopCoroutine("fadeInPanel");
		StartCoroutine("fadeInPanel");
		yield return new WaitForSeconds(1.3f);
		//yield return new WaitForSeconds(2f);
		if(_map != null)
		{
			_map.IsVisible = true;
		}
	}

	IEnumerator fadeInPanel()
	{
		var spin = panelColour;
		float alph = 0;
		spin.a = alph;
		snapshotImage.color = spin;
		yield return new WaitForEndOfFrame();
		snapshotImage.gameObject.SetActive(true);
		frameImage.gameObject.SetActive(true);
		while(alph < 1)
		{
			alph = spin.a;
			alph += 0.03f;
			spin.a = alph;
			snapshotImage.color = spin;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		fadedIn = true;
	}

	IEnumerator fadeOutPanel()
	{
		var spin = panelColour;
		float alph = 1;
		spin.a = alph;
		snapshotImage.color = spin;
		yield return new WaitForEndOfFrame();
		snapshotImage.gameObject.SetActive(true);
		while(alph > 0)
		{
			alph = spin.a;
			alph -= 0.03f;

			spin.a = alph;
			snapshotImage.color = spin;

			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		snapshotImage.gameObject.SetActive(false);
		frameImage.gameObject.SetActive(false);
		fadedIn = false;
	}
}
