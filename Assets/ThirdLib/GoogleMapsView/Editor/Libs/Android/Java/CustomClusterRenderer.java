package com.deadmosquitogames.gmaps.clustering;

import android.content.Context;

import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.maps.android.clustering.ClusterManager;
import com.google.maps.android.clustering.view.DefaultClusterRenderer;

public class CustomClusterRenderer extends DefaultClusterRenderer<MyClusterItem> {
	public CustomClusterRenderer(Context context, GoogleMap map, ClusterManager<MyClusterItem> clusterManager) {
		super(context, map, clusterManager);
	}

	@Override
	protected void onBeforeClusterItemRendered(MyClusterItem item, MarkerOptions markerOptions) {
		MarkerOptions customOptions = item.getMarkerOptions();
		markerOptions
			.position(customOptions.getPosition())
				.icon(customOptions.getIcon())
				.alpha(customOptions.getAlpha())
				.anchor(customOptions.getAnchorU(), customOptions.getAnchorV())
				.infoWindowAnchor(customOptions.getInfoWindowAnchorU(), customOptions.getInfoWindowAnchorV())
				.draggable(customOptions.isDraggable())
				.flat(customOptions.isFlat())
				.rotation(customOptions.getRotation()) // Rotate marker a bit
				.snippet(customOptions.getSnippet())
				.title(customOptions.getTitle())
				.visible(customOptions.isVisible())
				.zIndex(customOptions.getZIndex());
	}
}
