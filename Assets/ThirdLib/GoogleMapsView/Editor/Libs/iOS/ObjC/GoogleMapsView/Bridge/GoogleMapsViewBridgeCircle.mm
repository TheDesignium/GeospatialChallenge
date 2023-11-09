
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import <GoogleMaps/GoogleMaps.h>
#import "CustomGMSCircle.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"

extern "C" {

extern char *_googleMapsViewCircleGetId(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return [GoogleMapsViewUtils createCStringFrom:circle.identifier];
}


extern char *_googleMapsViewCircleGetCenter(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    NSString *locationJson = [GoogleMapsJsonUtils serializeCoords:circle.position];
    return [GoogleMapsViewUtils createCStringFrom:locationJson];
}


extern void _googleMapsViewCircleSetCenter(void *circlePtr, double lat, double lng) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.position = CLLocationCoordinate2DMake(lat, lng);
}


extern char *_googleMapsViewCircleGetFillColor(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    NSString *colorJson = [GoogleMapsJsonUtils serializeColor:circle.fillColor];
    return [GoogleMapsViewUtils createCStringFrom:colorJson];
}


extern void _googleMapsViewCircleSetFillColor(void *circlePtr, float color[]) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.fillColor = [GoogleMapsViewUtils colorFromFloatArr:color];
}


extern char *_googleMapsViewCircleGetStrokeColor(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    NSString *colorJson = [GoogleMapsJsonUtils serializeColor:circle.strokeColor];
    return [GoogleMapsViewUtils createCStringFrom:colorJson];
}


extern void _googleMapsViewCircleSetStrokeColor(void *circlePtr, float color[]) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.strokeColor = [GoogleMapsViewUtils colorFromFloatArr:color];
}


extern float _googleMapsViewCircleGetStrokeWidth(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return circle.strokeWidth;
}


extern void _googleMapsViewCircleSetStrokeWidth(void *circlePtr, float width) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.strokeWidth = width;
}


extern int _googleMapsViewCircleGetZIndex(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return circle.zIndex;
}


extern void _googleMapsViewCircleSetZIndex(void *circlePtr, int zIndex) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.zIndex = zIndex;
}


extern bool _googleMapsViewCircleIsVisible(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return circle.map != nil;
}


extern void _googleMapsViewCircleSetVisible(void *circlePtr, void *mapPtr, bool visible) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);

    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        circle.map = map;
    } else {
        circle.map = nil;
    }
}


extern bool _googleMapsViewCircleIsClickable(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return circle.tappable;
}


extern void _googleMapsViewCircleSetClickable(void *circlePtr, bool clickable) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.tappable = clickable;
}


extern void _googleMapsViewCircleCircleRemove(void *circlePtr) {
    CustomGMSCircle *circle = (CustomGMSCircle *) CFBridgingRelease(circlePtr);
    circle.map = nil;
}


extern double _googleMapsViewCircleGetRadius(void *circlePtr) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    return circle.radius;
}


extern void _googleMapsViewCircleSetRadius(void *circlePtr, double radius) {
    CustomGMSCircle *circle = (__bridge CustomGMSCircle *) (circlePtr);
    circle.radius = radius;
}
}

#pragma clang diagnostic pop

