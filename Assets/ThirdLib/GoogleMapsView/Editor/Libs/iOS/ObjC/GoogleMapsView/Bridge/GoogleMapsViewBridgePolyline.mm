
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import "CustomGMSPolyline.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"

extern "C" {

extern char *_googleMapsViewPolylineGetId(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return [GoogleMapsViewUtils createCStringFrom:polyline.identifier];
}


extern char *_googleMapsViewPolylineGetPoints(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);

    NSString *pointsJson = [GoogleMapsJsonUtils serializePoints:polyline.path];
    return [GoogleMapsViewUtils createCStringFrom:pointsJson];
}


extern void _googleMapsViewPolylineSetPoints(void *polylinePtr, char *pointsList) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    NSString *pointsListJson = [GoogleMapsViewUtils createNSStringFrom:pointsList];
    NSArray *pointsArr = [GoogleMapsJsonUtils deserializeArray:pointsListJson];
    polyline.path = [GoogleMapsJsonUtils deserializePointsArr:pointsArr];
}


extern char *_googleMapsViewPolylineGetColor(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    NSString *colorJson = [GoogleMapsJsonUtils serializeColor:polyline.strokeColor];
    return [GoogleMapsViewUtils createCStringFrom:colorJson];
}


extern void _googleMapsViewPolylineSetColor(void *polylinePtr, float color[]) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    polyline.strokeColor = [GoogleMapsViewUtils colorFromFloatArr:color];
}


extern float _googleMapsViewPolylineGetWidth(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return polyline.strokeWidth;
}


extern void _googleMapsViewPolylineSetWidth(void *polylinePtr, float width) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    polyline.strokeWidth = width;
}


extern int _googleMapsViewPolylineGetZIndex(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return polyline.zIndex;
}


extern void _googleMapsViewPolylineSetZIndex(void *polylinePtr, int zIndex) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    polyline.zIndex = zIndex;
}


extern bool _googleMapsViewPolylineIsVisible(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return polyline.map != nil;
}


extern void _googleMapsViewPolylineSetVisible(void *polylinePtr, void *mapPtr, bool visible) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        polyline.map = map;
    } else {
        polyline.map = nil;
    }
}


extern bool _googleMapsViewPolylineIsGeodesic(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return polyline.geodesic;
}


extern void _googleMapsViewPolylineSetGeodesic(void *polylinePtr, bool isGeodesic) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    polyline.geodesic = isGeodesic;
}


extern bool _googleMapsViewPolylineIsClickable(void *polylinePtr) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    return polyline.tappable;
}


extern void _googleMapsViewPolylineSetClickable(void *polylinePtr, bool clickable) {
    CustomGMSPolyline *polyline = (__bridge CustomGMSPolyline *) (polylinePtr);
    polyline.tappable = clickable;
}


extern void _googleMapsViewPolylineRemove(void *polylinePtr) {
    CustomGMSPolyline *polyline = (CustomGMSPolyline *) CFBridgingRelease(polylinePtr);
    polyline.map = nil;
}
}

#pragma clang diagnostic pop

