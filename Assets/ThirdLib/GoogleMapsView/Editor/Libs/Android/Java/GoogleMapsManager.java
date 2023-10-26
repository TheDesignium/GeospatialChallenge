package com.deadmosquitogames.gmaps;

import android.app.Activity;
import android.content.Context;
import android.os.Build;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;

import com.deadmosquitogames.gmaps.clustering.MyClusterItem;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.GoogleMapOptions;
import com.google.android.gms.maps.MapView;
import com.google.android.gms.maps.MapsInitializer;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.OnMapsSdkInitializedCallback;
import com.google.android.gms.maps.model.Circle;
import com.google.android.gms.maps.model.CircleOptions;
import com.google.android.gms.maps.model.GroundOverlay;
import com.google.android.gms.maps.model.GroundOverlayOptions;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.gms.maps.model.Polygon;
import com.google.android.gms.maps.model.PolygonOptions;
import com.google.android.gms.maps.model.Polyline;
import com.google.android.gms.maps.model.PolylineOptions;
import com.google.android.gms.maps.model.TileOverlay;
import com.google.android.gms.maps.model.TileOverlayOptions;
import com.google.maps.android.clustering.ClusterManager;
import com.google.maps.android.heatmaps.Gradient;
import com.google.maps.android.heatmaps.HeatmapTileProvider;
import com.google.maps.android.heatmaps.WeightedLatLng;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

@SuppressWarnings("unused")
public class GoogleMapsManager implements OnMapsSdkInitializedCallback {

	public static final String TAG = GoogleMapsManager.class.getSimpleName();
	private GoogleMap _map;

	private Activity _activity;
	private MapView _mapView;
	private GoogleMap.OnCameraIdleListener _onCameraIdleListener;
	private GoogleMap.OnCameraIdleListener _clusterOnCameraIdleListener;
	private GoogleMap.OnMarkerDragListener onMarkerDragListener;

	public GoogleMapsManager(Activity activity) {
		_activity = activity;
		MapsInitializer.initialize(activity.getApplicationContext(), MapsInitializer.Renderer.LEGACY, this);
	}

	public GoogleMap getMap() {
		return _map;
	}

	public void show(int x, int y, int width, int height, GoogleMapOptions options, final OnMapReadyCallback onMapReadyCallback) {
		if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.N_MR1) {
			// Hack to fix older androids - unfortunately it makes map UI elements to fall behind
			options.zOrderOnTop(true);
		}
		_mapView = new MapView(_activity, options);
		_mapView.setTag("NINEVA");
		_mapView.onCreate(null);
		_mapView.onStart();
		_mapView.onResume();

		_mapView.setX(x);
		_mapView.setY(y);
		_mapView.setLayoutParams(new ViewGroup.LayoutParams(width, height));

		getContent().addView(_mapView);

		_mapView.getMapAsync(new OnMapReadyCallback() {
			@Override
			public void onMapReady(GoogleMap googleMap) {
				_map = googleMap;
				if (onMapReadyCallback != null) {
					onMapReadyCallback.onMapReady(_map);
				}
				_map.setOnMarkerDragListener(new GoogleMap.OnMarkerDragListener() {
					@Override
					public void onMarkerDragStart(Marker marker) {
						if (onMarkerDragListener != null) {
							onMarkerDragListener.onMarkerDragStart(marker);
						}
					}

					@Override
					public void onMarkerDrag(Marker marker) {
						if (onMarkerDragListener != null) {
							onMarkerDragListener.onMarkerDrag(marker);
						}
					}

					@Override
					public void onMarkerDragEnd(Marker marker) {
						if (onMarkerDragListener != null) {
							onMarkerDragListener.onMarkerDragEnd(marker);
						}
					}
				});
				_map.setOnCameraIdleListener(new GoogleMap.OnCameraIdleListener() {
					@Override
					public void onCameraIdle() {
						if (_onCameraIdleListener != null) {
							_onCameraIdleListener.onCameraIdle();
						}
						if (_clusterOnCameraIdleListener != null) {
							_clusterOnCameraIdleListener.onCameraIdle();
						}
					}
				});
			}
		});

	}

	public void setRect(int x, int y, int width, int height) {
		_mapView.setX(x);
		_mapView.setY(y);
		ViewGroup.LayoutParams layoutParams = _mapView.getLayoutParams();
		layoutParams.width = width;
		layoutParams.height = height;
		_mapView.setLayoutParams(layoutParams);
	}

	// region extra_api
	public Circle addCircle(CircleOptions circleOptions) {
		if (_map == null) {
			return null;
		}
		return _map.addCircle(circleOptions);
	}


	public GroundOverlay addGroundOverlay(GroundOverlayOptions groundOverlayOptions) {
		if (_map == null) {
			return null;
		}

		return _map.addGroundOverlay(groundOverlayOptions);
	}


	public Marker addMarker(MarkerOptions markerOptions) {
		if (_map == null) {
			return null;
		}
		return _map.addMarker(markerOptions);
	}


	public Polygon addPolygon(PolygonOptions polygonOptions) {
		if (_map == null) {
			return null;
		}
		return _map.addPolygon(polygonOptions);
	}


	public Polyline addPolyline(PolylineOptions polylineOptions) {
		if (_map == null) {
			return null;
		}
		return _map.addPolyline(polylineOptions);
	}


	public TileOverlay addTileOverlay(TileOverlayOptions tileOverlayOptions) {
		if (_map == null) {
			return null;
		}
		return _map.addTileOverlay(tileOverlayOptions);
	}
	//endregion


	public void clear() {
		if (_map == null) {
			return;
		}

		_map.clear();
	}


	public void dismiss() {
		if (_mapView != null) {
			ViewGroup view = getContent();
			view.removeView(_mapView);
			_mapView = null;
		}
	}


	public MapView getMapView() {
		return _mapView;
	}


	public void setVisible(boolean visible) {
		if (_mapView != null) {
			ViewGroup view = getContent();
			if (visible) {
				if (_mapView.getParent() == null) {
					getContent().addView(_mapView);
				}
				_mapView.setVisibility(View.VISIBLE);
			} else {
				view.removeView(_mapView);
				_mapView.setVisibility(View.GONE);
			}
		}
	}

	private ViewGroup getContent() {
		return _activity.findViewById(android.R.id.content);
	}


	public boolean isVisible() {
		return _mapView != null && _mapView.getVisibility() == View.VISIBLE;
	}


	public void setOnCameraIdleListener(GoogleMap.OnCameraIdleListener listener) {
		_onCameraIdleListener = listener;
	}


	public void setClusterOnCameraIdleListener(GoogleMap.OnCameraIdleListener listener) {
		_clusterOnCameraIdleListener = listener;
	}

	public void setOnMarkerDragListener(GoogleMap.OnMarkerDragListener onMarkerDragListener) {
		this.onMarkerDragListener = onMarkerDragListener;
	}

	public static HeatmapTileProvider createHeatmapTileProvider(int radius, Gradient gradient, double opacity, String data) throws JSONException {
		List<WeightedLatLng> resultData = new ArrayList<>();
		JSONArray array = new JSONArray(data);
		for (int i = 0; i < array.length(); i++) {
			JSONObject jo = array.getJSONObject(i);
			LatLng pos = JsonUtil.parseLatLng(jo);
			resultData.add(new WeightedLatLng(pos, jo.getDouble("intensity")));
		}

		return new HeatmapTileProvider.Builder()
				.radius(radius)
				.gradient(gradient)
				.opacity(opacity)
				.weightedData(resultData)
				.build();
	}

	public static void addClusterItems(ClusterManager<MyClusterItem> clusterManager, String items, Context context) throws JSONException {
		Collection<MyClusterItem> myClusterItems = new ArrayList<>();
		JSONArray array = new JSONArray(items);

		for (int i = 0; i < array.length(); i++) {
			JSONObject jo = array.getJSONObject(i);
			MarkerOptions markerOptions = JsonUtil.parseMarkerOptions(jo, context);
			myClusterItems.add(new MyClusterItem(markerOptions));
		}
		clusterManager.addItems(myClusterItems);
	}

	@Override
	public void onMapsSdkInitialized(@NonNull MapsInitializer.Renderer renderer) {
		switch (renderer) {
			case LATEST:
				Log.d("GoogleMapsView", "The latest version of the renderer is used.");
				break;
			case LEGACY:
				Log.d("GoogleMapsView", "The legacy version of the renderer is used.");
				break;
		}
	}
}
