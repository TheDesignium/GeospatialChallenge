package com.deadmosquitogames.gmaps.clustering;

import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.maps.android.clustering.ClusterItem;

@SuppressWarnings("unused")
public class MyClusterItem implements ClusterItem {
	private MarkerOptions markerOptions;

	public MyClusterItem(MarkerOptions markerOptions) {
		this.markerOptions = markerOptions;
	}

	@Override
	public LatLng getPosition() {
		return markerOptions.getPosition();
	}

	@Override
	public String getTitle() {
		return markerOptions.getTitle();
	}

	@Override
	public String getSnippet() {
		return markerOptions.getSnippet();
	}

	public MarkerOptions getMarkerOptions() {
		return markerOptions;
	}
}