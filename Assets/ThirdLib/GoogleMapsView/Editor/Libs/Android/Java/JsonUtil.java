package com.deadmosquitogames.gmaps;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Base64;
import android.util.LruCache;

import com.google.android.gms.maps.model.BitmapDescriptor;
import com.google.android.gms.maps.model.BitmapDescriptorFactory;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import org.json.JSONException;
import org.json.JSONObject;

public class JsonUtil {

	private static final String PositionMethodName = "position";
	private static final String ZIndexMethodName = "zIndex";
	private static final String TitleMethodName = "title";
	private static final String SnippetMethodName = "snippet";
	private static final String DraggableMethodName = "draggable";
	private static final String VisibleMethodName = "visible";
	private static final String FlatMethodName = "flat";
	private static final String RotationMethodName = "rotation";
	private static final String AlphaMethodName = "alpha";
	private static final String AnchorMethodName = "anchor";
	private static final String InfoWindowAnchorMethodName = "infoWindowAnchor";
	private static final String IconMethodName = "iconAssetName";

	private static final LruCache<String, BitmapDescriptor> BitmapCache = new LruCache<>((int)(Runtime.getRuntime().maxMemory() / 1024 / 8));

	public static LatLng parseLatLng(JSONObject jsonObject) {
		try {
			return new LatLng(jsonObject.getDouble("lat"), jsonObject.getDouble("lng"));
		} catch (JSONException e) {
			e.printStackTrace();
			return null;
		}
	}

	public static MarkerOptions parseMarkerOptions(JSONObject jsonObject, Context context) {
		try {
			MarkerOptions markerOptions = new MarkerOptions();
			if (jsonObject.has(PositionMethodName)) {
				markerOptions.position(parseLatLng(jsonObject.getJSONObject(PositionMethodName)));
			}
			markerOptions.zIndex((float) jsonObject.getDouble(ZIndexMethodName));
			markerOptions.title(jsonObject.getString(TitleMethodName));
			markerOptions.snippet(jsonObject.getString(SnippetMethodName));
			markerOptions.draggable(jsonObject.getBoolean(DraggableMethodName));
			markerOptions.visible(jsonObject.getBoolean(VisibleMethodName));
			markerOptions.flat(jsonObject.getBoolean(FlatMethodName));
			markerOptions.rotation((float) jsonObject.getDouble(RotationMethodName));
			markerOptions.alpha((float) jsonObject.getDouble(AlphaMethodName));
			markerOptions.anchor((float) jsonObject.getDouble(AnchorMethodName + "U"), (float) jsonObject.getDouble(AnchorMethodName + "V"));
			markerOptions.infoWindowAnchor((float) jsonObject.getDouble(InfoWindowAnchorMethodName + "U"), (float) jsonObject.getDouble(InfoWindowAnchorMethodName + "V"));

			if (jsonObject.has("iconHue")) {
				markerOptions.icon(BitmapDescriptorFactory.defaultMarker((float) jsonObject.getDouble("iconHue")));
			} else if (jsonObject.has("iconBytes")) {
				String iconBytes = jsonObject.getString("iconBytes");

				String cacheKey = String.valueOf(iconBytes.hashCode());
				BitmapDescriptor descriptor = BitmapCache.get(cacheKey);
				if (descriptor == null) {
					byte[] decoded = Base64.decode(iconBytes, Base64.DEFAULT);
					Bitmap bitmap = BitmapFactory.decodeByteArray(decoded, 0, decoded.length);
					descriptor = BitmapDescriptorFactory.fromBitmap(bitmap);
					BitmapCache.put(cacheKey, descriptor);
				}

				markerOptions.icon(descriptor);
			} else if (jsonObject.has(IconMethodName)) {
				String fullPath = jsonObject.getString(IconMethodName);
				boolean isObbSplit = jsonObject.getBoolean("isObbSplit");

				BitmapDescriptor descriptor = BitmapCache.get(fullPath);
				if (descriptor == null) {
					descriptor = isObbSplit
							? BitmapDescriptorFactory.fromBitmap(ExpansionFileUtils.getBitmap(context, fullPath))
							: BitmapDescriptorFactory.fromAsset(fullPath);
					BitmapCache.put(fullPath, descriptor);
				}
				markerOptions.icon(descriptor);
			} else {
				markerOptions.icon(BitmapDescriptorFactory.defaultMarker());
			}
			return markerOptions;
		} catch (Exception e) {
			e.printStackTrace();
			return null;
		}
	}
}