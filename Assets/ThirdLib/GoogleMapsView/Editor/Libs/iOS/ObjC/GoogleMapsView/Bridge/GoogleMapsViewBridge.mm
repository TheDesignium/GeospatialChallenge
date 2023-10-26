
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import <GoogleMaps/GoogleMaps.h>
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"
#import "CustomGMSCircle.h"
#import "CustomGMSPolyline.h"
#import "CustomGMSPolygon.h"
#import "CustomGMSGroundOverlay.h"
#import "CustomGMSMarker.h"
#import "GoogleMapsViewDelegate.h"
#import "GoogleMapsFunctionDefs.h"
#import "GMUHeatmapTileLayer.h"
#import "CustomGMSTileOverlay.h"

CustomGMSMarker *createMarker(NSString *optionsStr);

extern "C" {

void _googleMapsViewInit(char *apiKey) {
    NSString *apiKeyStr = [GoogleMapsViewUtils createNSStringFrom:apiKey];
    [GMSServices provideAPIKey:apiKeyStr];
}

void *_createGoogleMapsView() {
    UIView *rootView = UnityGetGLView();
    CGRect frame = CGRectMake(rootView.frame.origin.x, rootView.frame.origin.x, rootView.frame.size.width / 2, rootView.frame.size.height / 2);

    GMSCameraPosition *camera = [GMSCameraPosition cameraWithLatitude:-33.86
                                                            longitude:151.20
                                                                 zoom:6];

    GMSMapView *mapView = [GMSMapView mapWithFrame:frame camera:camera];
    mapView.delegate = [GoogleMapsViewDelegate instance];

    return (void *) CFBridgingRetain(mapView);
}

void _googleMapsViewSetRect(void *mapPtr, int x, int y, int width, int height) {
    GMSMapView *mapView = (__bridge GMSMapView *) (mapPtr);

    CGRect frame = [GoogleMapsViewUtils scaledRectWith:x y:y width:width height:height];
    mapView.frame = frame;
}

void _googleMapsViewShow(void *mapPtr, int x, int y, int width, int height, char *options) {
    GMSMapView *mapView = (__bridge GMSMapView *) (mapPtr);
    mapView.settings.consumesGesturesInView = NO;

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];

    mapView.mapType = (GMSMapViewType) [optionsDic[@"mapType"] intValue];

    GMSCameraPosition *camera = [GoogleMapsJsonUtils deserializeCameraFrom:optionsDic];
    mapView.camera = camera;

    NSString *camBoundsKey = @"latLngBoundsForCameraTarget";
    if (optionsDic[camBoundsKey]) {
        mapView.cameraTargetBounds = [GoogleMapsJsonUtils deserializeBounds:optionsDic[camBoundsKey]];
    }

    NSString *minZoomKey = @"minZoomPreference";
    NSString *maxZoomKey = @"maxZoomPreference";
    if (optionsDic[minZoomKey]) {
        float minZoom = [optionsDic[minZoomKey] floatValue];
        [mapView setMinZoom:minZoom maxZoom:mapView.maxZoom];
    }
    if (optionsDic[maxZoomKey]) {
        float maxZoom = [optionsDic[maxZoomKey] floatValue];
        [mapView setMinZoom:mapView.minZoom maxZoom:maxZoom];
    }

    mapView.settings.compassButton = [optionsDic[@"compassEnabled"] boolValue];
    mapView.settings.rotateGestures = [optionsDic[@"rotateGesturesEnabled"] boolValue];
    mapView.settings.zoomGestures = [optionsDic[@"zoomGesturesEnabled"] boolValue];
    mapView.settings.tiltGestures = [optionsDic[@"tiltGesturesEnabled"] boolValue];
    mapView.settings.scrollGestures = [optionsDic[@"scrollGesturesEnabled"] boolValue];

    CGRect frame = [GoogleMapsViewUtils scaledRectWith:x y:y width:width height:height];
    mapView.frame = frame;

    [UnityGetGLView() addSubview:mapView];
}

void _googleMapsViewRemove(void *mapPtr) {
    GMSMapView *mapViewRef = (GMSMapView *) CFBridgingRelease(mapPtr);
    [mapViewRef removeFromSuperview];
}

bool _googleMapsViewIsVisible(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    return !mapViewRef.hidden;
}

void _googleMapsViewSetVisible(void *mapPtr, bool visible) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    mapViewRef.hidden = !visible;
}

bool _googleMapsViewIsTrafficEnabled(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    return mapViewRef.trafficEnabled;
}

void _googleMapsViewSetTrafficEnabled(void *mapPtr, bool trafficEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    mapViewRef.trafficEnabled = trafficEnabled;
}

int _googleMapsViewGetMapType(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    return mapViewRef.mapType;
}

void _googleMapsViewSetMapType(void *mapPtr, int mapType) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    mapViewRef.mapType = (GMSMapViewType) mapType;
}

char *_googleMapsViewGetCameraPosition(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    NSString *cameraJson = [GoogleMapsJsonUtils serializeCameraPosition:mapViewRef.camera];
    return [GoogleMapsViewUtils createCStringFrom:cameraJson];
}

bool _googleMapsViewSetStyle(void *mapPtr, char *json) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *jsonStr = [GoogleMapsViewUtils createNSStringFrom:json];

    NSError *error = nil;
    GMSMapStyle *style = [GMSMapStyle styleWithJSONString:jsonStr error:&error];
    [mapViewRef setMapStyle:style];

    return error == nil;
}

void _googleMapsViewAnimateCamera(void *mapPtr, char *animateCamera, bool isAnimated) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *animateCameraStr = [GoogleMapsViewUtils createNSStringFrom:animateCamera];
    NSDictionary *dic = [GoogleMapsJsonUtils deserializeDictionary:animateCameraStr];
    NSString *type = dic[@"type"];

    GMSCameraUpdate *cameraUpdate;

    if ([type isEqualToString:@"newCameraPosition"]) {
        NSDictionary *cameraPosDic = dic[@"cameraPosition"];
        double latitude = [cameraPosDic[@"cameraPositionLat"] doubleValue];
        double longitude = [cameraPosDic[@"cameraPositionLng"] doubleValue];
        float zoom = [cameraPosDic[@"cameraPositionZoom"] floatValue];
        double bearing = [cameraPosDic[@"cameraPositionBearing"] floatValue];
        double angle = [cameraPosDic[@"cameraPositionTilt"] floatValue];
        GMSCameraPosition *cameraPos = [GMSCameraPosition cameraWithLatitude:latitude longitude:longitude zoom:zoom bearing:bearing viewingAngle:angle];
        cameraUpdate = [GMSCameraUpdate setCamera:cameraPos];
    } else if ([type isEqualToString:@"newLatLng"]) {
        double lat = [dic[@"lat"] doubleValue];
        double lng = [dic[@"lng"] doubleValue];
        CLLocationCoordinate2D target = CLLocationCoordinate2DMake(lat, lng);
        cameraUpdate = [GMSCameraUpdate setTarget:target];
    } else if ([type isEqualToString:@"newLatLngZoom"]) {
        double lat = [dic[@"lat"] doubleValue];
        double lng = [dic[@"lng"] doubleValue];
        float zoom = [dic[@"zoom"] floatValue];
        CLLocationCoordinate2D target = CLLocationCoordinate2DMake(lat, lng);
        cameraUpdate = [GMSCameraUpdate setTarget:target zoom:zoom];
    } else if ([type isEqualToString:@"newLatLngBounds"]) {
        NSDictionary *boundsDic = dic[@"bounds"];
        GMSCoordinateBounds *bounds = [GoogleMapsJsonUtils deserializeBounds:boundsDic];
        float padding = [dic[@"padding"] floatValue];
        cameraUpdate = [GMSCameraUpdate fitBounds:bounds withPadding:padding];
    } else if ([type isEqualToString:@"newLatLngBoundsWithInsets"]) {
        NSDictionary *boundsDic = dic[@"bounds"];
        GMSCoordinateBounds *bounds = [GoogleMapsJsonUtils deserializeBounds:boundsDic];
        float padding = [dic[@"padding"] floatValue];
        cameraUpdate = [GMSCameraUpdate fitBounds:bounds withEdgeInsets:UIEdgeInsetsMake(padding, padding, padding, padding)];
    } else if ([type isEqualToString:@"zoomIn"]) {
        cameraUpdate = [GMSCameraUpdate zoomIn];
    } else if ([type isEqualToString:@"zoomOut"]) {
        cameraUpdate = [GMSCameraUpdate zoomOut];
    } else if ([type isEqualToString:@"zoomBy"]) {
        float zoomBy = [dic[@"zoom"] floatValue];
        cameraUpdate = [GMSCameraUpdate zoomBy:zoomBy];
    } else if ([type isEqualToString:@"zoomByXY"]) {
        float zoomBy = [dic[@"zoom"] floatValue];
        float x = [dic[@"x"] floatValue];
        float y = [dic[@"y"] floatValue];
        cameraUpdate = [GMSCameraUpdate zoomBy:zoomBy atPoint:CGPointMake(x, y)];
    } else if ([type isEqualToString:@"zoomTo"]) {
        float zoom = [dic[@"zoom"] floatValue];
        cameraUpdate = [GMSCameraUpdate zoomTo:zoom];
    } else if ([type isEqualToString:@"scrollBy"]) {
        float x = [dic[@"x"] floatValue];
        float y = [dic[@"y"] floatValue];
        cameraUpdate = [GMSCameraUpdate scrollByX:x Y:y];
    }

    if (isAnimated) {
        [mapViewRef animateWithCameraUpdate:cameraUpdate];
    } else {
        [mapViewRef moveCamera:cameraUpdate];
    }
}

void* _googleMapsViewGetProjection(void* mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    return (__bridge void *) [mapViewRef projection];
}

#pragma mark drawing

void *_googleMapsViewAddCircle(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];
    NSDictionary *centerDic = optionsDic[@"center"];

    CustomGMSCircle *circle = [CustomGMSCircle circleWithIdentifier:[NSUUID UUID].UUIDString];
    circle.position = CLLocationCoordinate2DMake([centerDic[@"lat"] doubleValue], [centerDic[@"lng"] doubleValue]);
    circle.radius = [optionsDic[@"radius"] doubleValue];
    circle.strokeWidth = [optionsDic[@"strokeWidth"] floatValue];
    circle.fillColor = [GoogleMapsJsonUtils deserializeColor:optionsDic[@"fillColor"]];
    circle.strokeColor = [GoogleMapsJsonUtils deserializeColor:optionsDic[@"strokeColor"]];
    circle.map = mapViewRef;
    circle.zIndex = [optionsDic[@"zIndex"] intValue];
    circle.tappable = [optionsDic[@"clickable"] boolValue];

    return (void *) CFBridgingRetain(circle);
}

void _googleMapsViewSetPadding(void *mapPtr, int left, int top, int right, int bottom) {
    UIEdgeInsets mapInsets = UIEdgeInsetsMake(top, left, bottom, right);
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    mapViewRef.padding = mapInsets;
}

void *_googleMapsViewAddMarker(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];
    CustomGMSMarker *marker = [GoogleMapsJsonUtils deserializeMarkerOptions:optionsDic withCache:[GoogleMapsViewUtils cache]];

    marker.map = mapViewRef;

    return (void *) CFBridgingRetain(marker);
}

void *_googleMapsViewAddGroundOverlay(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];

    CustomGMSGroundOverlay *overlay = [CustomGMSGroundOverlay new];

    UIImage *icon;
    
    NSString *iconKey = @"icon";
    if (optionsDic[iconKey]) {
        NSData *imageData = [NSData dataWithContentsOfFile:optionsDic[iconKey]];
        UIImage *overlayImage = [UIImage imageWithData:imageData];
        icon = overlayImage;
    } else if (optionsDic[@"iconBytes"]) {
        NSString *iconBytes = optionsDic[@"iconBytes"];
        NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:iconBytes options:0];
        icon = [UIImage imageWithData:decodedData];
    }

    if (icon == nil) {
        NSLog(@"Image was not found, please make sure it exists in your StreamingAssets folder. Debug info: %@", optionsStr);
    }

    NSString *boundsKey = @"bounds";
    if (optionsDic[boundsKey]) {
        GMSCoordinateBounds *bounds = [GoogleMapsJsonUtils deserializeBounds:optionsDic[boundsKey]];
        overlay = [CustomGMSGroundOverlay groundOverlayWithBounds:bounds icon:icon];
    }

    if (optionsDic[@"position"]) {
        CLLocationCoordinate2D position = [GoogleMapsJsonUtils coordFromDictionary:optionsDic[@"position"]];
        float zoomLevel = [optionsDic[@"zoomLevel"] floatValue];

        overlay = [CustomGMSGroundOverlay groundOverlayWithPosition:position icon:icon zoomLevel:zoomLevel];
    }

    overlay.identifier = [NSUUID UUID].UUIDString;
    overlay.anchor = CGPointMake([optionsDic[@"anchorU"] floatValue], [optionsDic[@"anchorV"] floatValue]);
    overlay.bearing = [optionsDic[@"bearing"] floatValue];
    overlay.icon = icon;
    overlay.opacity = 1.0f - [optionsDic[@"transparency"] floatValue];
    overlay.zIndex = [optionsDic[@"zIndex"] intValue];
    overlay.tappable = [optionsDic[@"clickable"] boolValue];

    overlay.map = mapViewRef;

    return (void *) CFBridgingRetain(overlay);
}

void *_googleMapsViewAddPolyline(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    CustomGMSPolyline *polyline = [CustomGMSPolyline polylineWithIdentifier:[NSUUID UUID].UUIDString];

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];

    polyline.path = [GoogleMapsJsonUtils deserializePointsArr:optionsDic[@"points"]];
    polyline.zIndex = [optionsDic[@"zIndex"] intValue];
    polyline.tappable = [optionsDic[@"clickable"] boolValue];
    polyline.geodesic = [optionsDic[@"geodesic"] boolValue];
    polyline.strokeColor = [GoogleMapsJsonUtils deserializeColor:optionsDic[@"color"]];
    polyline.strokeWidth = [optionsDic[@"width"] floatValue];

    polyline.map = mapViewRef;

    return (void *) CFBridgingRetain(polyline);
}

void *_googleMapsViewAddPolygon(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];

    CustomGMSPolygon *polygon = [CustomGMSPolygon polygonWithIdentifier:[NSUUID UUID].UUIDString];

    polygon.path = [GoogleMapsJsonUtils deserializePointsArr:optionsDic[@"points"]];
    polygon.zIndex = [optionsDic[@"zIndex"] intValue];
    polygon.tappable = [optionsDic[@"clickable"] boolValue];
    polygon.geodesic = [optionsDic[@"geodesic"] boolValue];
    polygon.strokeColor = [GoogleMapsJsonUtils deserializeColor:optionsDic[@"strokeColor"]];
    polygon.strokeWidth = [optionsDic[@"strokeWidth"] floatValue];
    polygon.fillColor = [GoogleMapsJsonUtils deserializeColor:optionsDic[@"fillColor"]];
    polygon.holes = [GoogleMapsJsonUtils deserializeHoles:optionsDic[@"holes"]];

    polygon.map = mapViewRef;

    return (void *) CFBridgingRetain(polygon);
}

static NSMutableArray<GMUWeightedLatLng *> *getHeatmapData(NSDictionary *tileProviderDic) {
    NSMutableArray<GMUWeightedLatLng *> *finalHeatmapData = [NSMutableArray new];

    NSArray *deserializedArr = tileProviderDic[@"weightedData"];
    for (NSDictionary *dic in deserializedArr) {
        CLLocationCoordinate2D coord = [GoogleMapsJsonUtils deserializeCoord:dic];
        double intensity = [dic[@"intensity"] doubleValue];
        GMUWeightedLatLng *gmuWeightedLatLng = [[GMUWeightedLatLng alloc] initWithCoordinate:coord intensity:intensity];
        [finalHeatmapData addObject:gmuWeightedLatLng];
    }
    return finalHeatmapData;
}

void *_googleMapsViewTileOverlay(void *mapPtr, char *options) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);

    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:options];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];

    CustomGMSTileOverlay *tileLayer = [CustomGMSTileOverlay tileLayerWithIdentifier:[NSUUID UUID].UUIDString];

    // heatmap params
    NSDictionary *tileProviderDic = optionsDic[@"tileProvider"];
    NSMutableArray<GMUWeightedLatLng *> *finalHeatmapData = getHeatmapData(tileProviderDic);
    tileLayer.weightedData = finalHeatmapData;
    tileLayer.radius = [tileProviderDic[@"radius"] integerValue];

    // gradient
    NSDictionary *gradientDic = tileProviderDic[@"gradient"];
    NSArray<UIColor *> *colors = [GoogleMapsJsonUtils deserializeColorArray:gradientDic[@"colors"]];
    NSArray<NSNumber *> *points = [GoogleMapsJsonUtils deserializeNumbersArray:gradientDic[@"startPoints"]];
    NSUInteger size = [gradientDic[@"colorMapSize"] integerValue];
    tileLayer.gradient = [[GMUGradient alloc] initWithColors:colors startPoints:points colorMapSize:size];

    // overlay params
    tileLayer.opacity = 1.0f - [optionsDic[@"transparency"] floatValue];
    tileLayer.zIndex = [optionsDic[@"zIndex"] intValue];
    tileLayer.fadeIn = [optionsDic[@"fadeIn"] boolValue];

    if ([optionsDic[@"visible"] boolValue]) {
        tileLayer.map = mapViewRef;
    }

    return (void *) CFBridgingRetain(tileLayer);
}

void _googleMapsViewClear(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    [mapViewRef clear];
}

void _googleMapsViewTakeSnapshot(void *mapPtr, ImageResultDelegate callback, void *callbackPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    UIGraphicsBeginImageContextWithOptions(mapViewRef.bounds.size, false, [UIScreen mainScreen].scale);
    [mapViewRef.layer renderInContext:UIGraphicsGetCurrentContext()];
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();

    NSData *pictureData = UIImagePNGRepresentation(image);
    callback(callbackPtr, pictureData.bytes, pictureData.length);
    pictureData = nil;
}

#pragma mark location

void _googleMapsViewSetMyLocationEnabled(void *mapPtr, bool enabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    mapViewRef.myLocationEnabled = enabled;
}


bool _googleMapsViewGetMyLocationEnabled(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    return mapViewRef.isMyLocationEnabled;
}

char *_googleMapsViewGetMyLocation(void *mapPtr) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapPtr);
    CLLocation *location = mapViewRef.myLocation;
    NSString *serializedLocation = [GoogleMapsJsonUtils serializeLocation:location];
    return [GoogleMapsViewUtils createCStringFrom:serializedLocation];
}

#pragma mark ui_settings
// COMPASS
bool _googleMapsViewSettingsIsCompassEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.compassButton;
}

void _googleMapsViewSettingsSetCompassEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.compassButton = isEnabled;
}

// INDOOR PICKER
bool _googleMapsViewSettingsIsIndoorPickerEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.indoorPicker;
}

void _googleMapsViewSettingsSetIndoorPickerEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.indoorPicker = isEnabled;
}

// MY LOCATION
bool _googleMapsViewSettingsIsMyLocationEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.myLocationButton;
}

void _googleMapsViewSettingsSetMyLocationEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.myLocationButton = isEnabled;
}

// ROTATE GESTURES
bool _googleMapsViewSettingsIsRotateGesturesEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.rotateGestures;
}

void _googleMapsViewSettingsSetRotateGesturesEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.rotateGestures = isEnabled;
}

// SCROLL GESTURES
bool _googleMapsViewSettingsIsScrollGesturesEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.scrollGestures;
}

void _googleMapsViewSettingsSetScrollGesturesEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.scrollGestures = isEnabled;
}

// TILT GESTURES
bool _googleMapsViewSettingsIsTiltGesturesEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.tiltGestures;
}

void _googleMapsViewSettingsSetTiltGesturesEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.tiltGestures = isEnabled;
}

// ZOOM GESTURES
bool _googleMapsViewSettingsIsZoomGesturesEnabled(void *mapView) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    return mapViewRef.settings.zoomGestures;
}

void _googleMapsViewSettingsSetZoomGesturesEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    mapViewRef.settings.zoomGestures = isEnabled;
}

// Disable all gestures
void _googleMapsViewSettingsSetAllGesturesEnabled(void *mapView, bool isEnabled) {
    GMSMapView *mapViewRef = (__bridge GMSMapView *) (mapView);
    [mapViewRef.settings setAllGesturesEnabled:isEnabled];
}

#pragma mark listeners
void _googleMapsViewSetOnMapClickListener(OnLocationSelectedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.mapTapped = ^(CLLocationCoordinate2D location) {
        onClickCallback(onSuccessActionPtr, location.latitude, location.longitude);
    };
}

void _googleMapsViewSetOnMapLongClickListener(OnLocationSelectedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.mapLongTapped = ^(CLLocationCoordinate2D location) {
        onClickCallback(onSuccessActionPtr, location.latitude, location.longitude);
    };
}

void _googleMapsViewSetOnMarkerClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr, bool defaultClickBehaviour) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.markerTapped = ^(CustomGMSMarker *marker) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) marker, (__bridge void *) (marker.map));
        return defaultClickBehaviour;
    };
}

void _googleMapsViewSetOnMarkerInfoWindowClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.markerInfoWindowTapped = ^(CustomGMSMarker *marker) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) marker, (__bridge void *) (marker.map));
    };
}
    
void _googleMapsViewSetOnMarkerDragListener(OnItemClickedDelegate dragCallback, void *onStartDragPtr, void *onEndDragPtr, void *onDragPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    
    mapDelegate.markerBeginDrag = ^(CustomGMSMarker *marker) {
        dragCallback(onStartDragPtr, (__bridge void *) marker, (__bridge void *) (marker.map));
    };
    mapDelegate.markerDrag = ^(CustomGMSMarker *marker) {
        dragCallback(onDragPtr, (__bridge void *) marker, (__bridge void *) (marker.map));
    };
    mapDelegate.markerEndDrag = ^(CustomGMSMarker *marker) {
        dragCallback(onEndDragPtr, (__bridge void *) marker, (__bridge void *) (marker.map));
    };
}

void _googleMapsViewSetOnCircleClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.circleTapped = ^(CustomGMSCircle *circle) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) circle, (__bridge void *) (circle.map));
    };
}

void _googleMapsViewSetOnGroundOverlayClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.groundOverlayTapped = ^(CustomGMSGroundOverlay *overlay) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) overlay, (__bridge void *) (overlay.map));
    };
}

void _googleMapsViewSetOnPolylineClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.polylineTapped = ^(CustomGMSPolyline *polyline) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) polyline, (__bridge void *) (polyline.map));
    };
}

void _googleMapsViewSetOnPolygonClickListener(OnItemClickedDelegate onClickCallback, void *onSuccessActionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.polygonTapped = ^(CustomGMSPolygon *polygon) {
        onClickCallback(onSuccessActionPtr, (__bridge void *) polygon, (__bridge void *) (polygon.map));
    };
}

void _googleMapsViewSetOnCameraMoveStartedListener(ActionIntCallbackDelegate onMoveStartedCallback, void *actionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.cameraStartedMoving = ^(int reason) {
        onMoveStartedCallback(actionPtr, reason);
    };
}

void _googleMapsViewSetOnCameraIdleListener(ActionVoidCallbackDelegate callback, void *actionPtr) {
    GoogleMapsViewDelegate *mapDelegate = [GoogleMapsViewDelegate instance];
    mapDelegate.cameraIdle = ^{
        callback(actionPtr);
    };
}

#pragma mark projection

char* _googleMapViewProjectionFromScreenLocation(void* projectionPointer, int x, int y) {
    GMSProjection *projection = (__bridge GMSProjection *) (projectionPointer);
    CLLocationCoordinate2D latLng = [projection coordinateForPoint:CGPointMake(x, y)];
    return [GoogleMapsViewUtils createCStringFrom:[GoogleMapsJsonUtils serializeCoords:latLng]];
}

char* _googleMapViewProjectionGetVisibleRegion(void* projectionPointer) {
    GMSProjection *projection = (__bridge GMSProjection *) (projectionPointer);
    GMSVisibleRegion region = [projection visibleRegion];
    NSMutableDictionary *regionDic = [[NSMutableDictionary alloc] init];
    regionDic[@"nearLeft"] = [GoogleMapsJsonUtils serializeCoords:region.nearLeft];
    regionDic[@"nearRight"] = [GoogleMapsJsonUtils serializeCoords:region.nearRight];
    regionDic[@"farLeft"] = [GoogleMapsJsonUtils serializeCoords:region.farLeft];
    regionDic[@"farRight"] = [GoogleMapsJsonUtils serializeCoords:region.farRight];
    GMSCoordinateBounds* bounds = [[GMSCoordinateBounds alloc] initWithRegion:region];
    regionDic[@"latLngBounds"] = [GoogleMapsJsonUtils serializeBounds:bounds];
    return [GoogleMapsViewUtils createCStringFrom:[GoogleMapsJsonUtils serializeDictionary:regionDic]];
}

char* _googleMapViewProjectionToScreenLocation(void* projectionPointer, char*  latLngJson) {
    GMSProjection *projection = (__bridge GMSProjection *) (projectionPointer);
    CLLocationCoordinate2D coord = [GoogleMapsJsonUtils deserializeCoord:[GoogleMapsJsonUtils deserializeDictionary:[GoogleMapsViewUtils createNSStringFrom:latLngJson]]];
    CGPoint point = [projection pointForCoordinate:coord];
    return [GoogleMapsViewUtils createCStringFrom:[[NSString alloc] initWithFormat:@"%f;%f", point.x, point.y]];
}

}

#pragma clang diagnostic pop

