package com.deadmosquitogames.gmaps;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;
import com.deadmosquitogames.gmaps.expansion.APKExpansionSupport;
import com.deadmosquitogames.gmaps.expansion.ZipResourceFile;

import java.io.InputStream;

@SuppressWarnings("unused")
public class ExpansionFileUtils {

	private static final String LOG_TAG = ExpansionFileUtils.class.getSimpleName();

	public static Bitmap getBitmap(Context context, String assetPath) throws Exception {
		InputStream inputStream = null;

		PackageManager manager = context.getPackageManager();
		PackageInfo info = manager.getPackageInfo(context.getPackageName(), 0);

		ZipResourceFile expansionFile = APKExpansionSupport.getAPKExpansionZipFile(context, info.versionCode, info.versionCode);

		// convert absolute jar path to relative one for zip
		String zipAssetPath = removeLeadingSlash(assetPath);

		if (expansionFile != null) {
			inputStream = expansionFile.getInputStream("assets/" + zipAssetPath);
		}

		if (inputStream == null) {
			Log.e(LOG_TAG, String.format("Unable to locate asset '%s' inside expansion file", zipAssetPath));
		}

		return BitmapFactory.decodeStream(inputStream);
	}

	public static String removeLeadingSlash(String path) {
		return path.startsWith("/") ? path.substring(1) : path;
	}
}
