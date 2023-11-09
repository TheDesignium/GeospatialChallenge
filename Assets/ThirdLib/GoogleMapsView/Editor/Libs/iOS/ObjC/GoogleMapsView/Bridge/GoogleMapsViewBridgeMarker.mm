
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import "CustomGMSMarker.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"

extern "C" {

extern char *_googleMapsViewMarkerGetId(void *markerPtr) {
    GMSMarker *marker = (__bridge GMSMarker *) (markerPtr);
    if ([marker isKindOfClass:[CustomGMSMarker class]]) {
        return [GoogleMapsViewUtils createCStringFrom:((CustomGMSMarker *) marker).identifier];
    } else {
        return nil;
    }
}

extern char *_googleMapsViewMarkerGetPosition(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    NSString *posStr = [GoogleMapsJsonUtils serializeCoords:marker.position];
    return [GoogleMapsViewUtils createCStringFrom:posStr];
}

extern void _googleMapsViewMarkerSetPosition(void *markerPtr, double lat, double lng) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.position = CLLocationCoordinate2DMake(lat, lng);
}

extern float _googleMapsViewMarkerGetTransparency(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.opacity;
}

extern void _googleMapsViewMarkerSetTransparency(void *markerPtr, float opacity) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.opacity = opacity;
}

extern void _googleMapsViewMarkerSetAnchor(void *markerPtr, float x, float y) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.groundAnchor = CGPointMake(x, y);
}

extern void _googleMapsViewMarkerSetInfoWindowAnchor(void *markerPtr, float x, float y) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.infoWindowAnchor = CGPointMake(x, y);
}

extern char *_googleMapsViewMarkerGetTitle(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return [GoogleMapsViewUtils createCStringFrom:marker.title];
}

extern void _googleMapsViewMarkerSetTitle(void *markerPtr, char *title) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.title = [GoogleMapsViewUtils createNSStringFrom:title];
}

extern char *_googleMapsViewMarkerGetSnippet(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return [GoogleMapsViewUtils createCStringFrom:marker.snippet];
}

extern void _googleMapsViewMarkerSetSnippet(void *markerPtr, char *snippet) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.snippet = [GoogleMapsViewUtils createNSStringFrom:snippet];
}

extern void _googleMapsViewMarkerSetIcon(void *markerPtr, char *imageDescriptor) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    NSString *optionsStr = [GoogleMapsViewUtils createNSStringFrom:imageDescriptor];
    NSDictionary *optionsDic = [GoogleMapsJsonUtils deserializeDictionary:optionsStr];
    
    [GoogleMapsJsonUtils setIconWithCache:[GoogleMapsViewUtils cache] marker:marker markerOptionsDic:optionsDic];
}

extern bool _googleMapsViewMarkerIsVisible(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.map != nil;
}

extern void _googleMapsViewMarkerSetVisible(void *markerPtr, void *mapPtr, bool visible) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        marker.map = map;
    } else {
        marker.map = nil;
    }
}

extern bool _googleMapsViewMarkerIsDraggable(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.isDraggable;
}

extern void _googleMapsViewMarkerSetDraggable(void *markerPtr, bool draggable) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.draggable = draggable;
}

extern bool _googleMapsViewMarkerIsFlat(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.isFlat;
}

extern void _googleMapsViewMarkerSetFlat(void *markerPtr, bool isFlat) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.flat = isFlat;
}

extern double _googleMapsViewMarkerGetRotation(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.rotation;
}

extern void _googleMapsViewMarkerSetRotation(void *markerPtr, double rotation) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.rotation = rotation;
}

extern void _googleMapsViewMarkerShowInfoWindow(void *markerPtr, void *mapPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
    map.selectedMarker = marker;
}

extern void _googleMapsViewMarkerHideInfoWindow(void *mapPtr) {
    GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
    map.selectedMarker = nil;
}

extern bool _googleMapsViewMarkerIsInfoWindowShown(void *markerPtr, void *mapPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
    if (map.selectedMarker == marker) {
	return true;
    }
    return false;
}

extern int _googleMapsViewMarkerGetZIndex(void *markerPtr) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    return marker.zIndex;
}

extern void _googleMapsViewMarkerSetZIndex(void *markerPtr, int bearing) {
    CustomGMSMarker *marker = (__bridge CustomGMSMarker *) (markerPtr);
    marker.zIndex = bearing;
}


extern void _googleMapsViewMarkerRemove(void *markerPtr) {
    CustomGMSMarker *marker = (CustomGMSMarker *) CFBridgingRelease(markerPtr);
    marker.map = nil;
}

}

#pragma clang diagnostic pop

