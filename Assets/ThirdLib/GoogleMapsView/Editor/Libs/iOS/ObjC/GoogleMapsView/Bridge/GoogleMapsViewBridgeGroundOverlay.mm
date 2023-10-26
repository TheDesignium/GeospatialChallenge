
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import "CustomGMSGroundOverlay.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"

extern "C" {

extern char *_googleMapsViewOverlayGetId(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return [GoogleMapsViewUtils createCStringFrom:overlay.identifier];
}

extern void _googleMapsViewOverlaySetImage(void *overlayPtr, char *imagePath) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.icon = [UIImage imageWithContentsOfFile:[GoogleMapsViewUtils createNSStringFrom:imagePath]];
}

extern int _googleMapsViewOverlayGetZIndex(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return overlay.zIndex;
}


extern void _googleMapsViewOverlaySetZIndex(void *overlayPtr, int bearing) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.zIndex = bearing;
}

extern double _googleMapsViewOverlayGetBearing(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return overlay.bearing;
}

extern void _googleMapsViewOverlaySetBearing(void *overlayPtr, double bearing) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.bearing = bearing;
}

extern float _googleMapsViewOverlayGetTransparency(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return 1.0f - overlay.opacity;
}

extern void _googleMapsViewOverlaySetTransparency(void *overlayPtr, float transparency) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.opacity = 1.0f - transparency;
}

extern char *_googleMapsViewOverlayGetBounds(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    NSString *boundsStr = [GoogleMapsJsonUtils serializeBounds:overlay.bounds];
    return [GoogleMapsViewUtils createCStringFrom:boundsStr];
}

extern void _googleMapsViewOverlaySetBounds(void *overlayPtr, char *bounds) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    NSString *boundsStr = [GoogleMapsViewUtils createNSStringFrom:bounds];
    NSDictionary *boundsDic = [GoogleMapsJsonUtils deserializeDictionary:boundsStr];
    overlay.bounds = [GoogleMapsJsonUtils deserializeBounds:boundsDic];
}

extern char *_googleMapsViewOverlayGetPosition(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    NSString *posStr = [GoogleMapsJsonUtils serializeCoords:overlay.position];
    return [GoogleMapsViewUtils createCStringFrom:posStr];
}

extern void _googleMapsViewOverlaySetPosition(void *overlayPtr, double lat, double lng) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.position = CLLocationCoordinate2DMake(lat, lng);
}

extern bool _googleMapsViewOverlayIsVisible(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return overlay.map != nil;
}

extern void _googleMapsViewOverlaySetVisible(void *overlayPtr, void *mapPtr, bool visible) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        overlay.map = map;
    } else {
        overlay.map = nil;
    }
}

extern bool _googleMapsViewOverlayIsClickable(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    return overlay.tappable;
}


extern void _googleMapsViewOverlaySetClickable(void *overlayPtr, bool clickable) {
    CustomGMSGroundOverlay *overlay = (__bridge CustomGMSGroundOverlay *) (overlayPtr);
    overlay.tappable = clickable;
}


extern void _googleMapsViewOverlayRemove(void *overlayPtr) {
    CustomGMSGroundOverlay *overlay = (CustomGMSGroundOverlay *) CFBridgingRelease(overlayPtr);
    overlay.map = nil;
}
}

#pragma clang diagnostic pop

