
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import "CustomGMSPolygon.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"

extern "C" {

extern char *_googleMapsViewPolygonGetId(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return [GoogleMapsViewUtils createCStringFrom:polygon.identifier];
}

extern char *_googleMapsViewPolygonGetPoints(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);

    NSString *pointsJson = [GoogleMapsJsonUtils serializePoints:polygon.path];
    return [GoogleMapsViewUtils createCStringFrom:pointsJson];
}


extern void _googleMapsViewPolygonSetPoints(void *polygonPtr, char *pointsList) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    NSString *pointsListJson = [GoogleMapsViewUtils createNSStringFrom:pointsList];
    NSArray *pointsArr = [GoogleMapsJsonUtils deserializeArray:pointsListJson];
    polygon.path = [GoogleMapsJsonUtils deserializePointsArr:pointsArr];
}


extern char *_googleMapsViewPolygonGetHoles(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    NSString *holesJson = [GoogleMapsJsonUtils serializeHoles:polygon.holes];
    return [GoogleMapsViewUtils createCStringFrom:holesJson];
}


extern void _googleMapsViewPolygonSetHoles(void *polygonPtr, char *holesList) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);

    NSString *holesListJson = [GoogleMapsViewUtils createNSStringFrom:holesList];
    NSArray *holesArr = [GoogleMapsJsonUtils deserializeArray:holesListJson];

    polygon.holes = [GoogleMapsJsonUtils deserializeHoles:holesArr];
}


extern char *_googleMapsViewPolygonGetStrokeColor(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    NSString *colorJson = [GoogleMapsJsonUtils serializeColor:polygon.strokeColor];
    return [GoogleMapsViewUtils createCStringFrom:colorJson];
}


extern void _googleMapsViewPolygonSetStrokeColor(void *polygonPtr, float color[]) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.strokeColor = [GoogleMapsViewUtils colorFromFloatArr:color];
}

extern char *_googleMapsViewPolygonGetFillColor(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    NSString *colorJson = [GoogleMapsJsonUtils serializeColor:polygon.fillColor];
    return [GoogleMapsViewUtils createCStringFrom:colorJson];
}


extern void _googleMapsViewPolygonSetFillColor(void *polygonPtr, float color[]) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.fillColor = [GoogleMapsViewUtils colorFromFloatArr:color];
}

extern float _googleMapsViewPolygonGetStrokeWidth(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return polygon.strokeWidth;
}


extern void _googleMapsViewPolygonSetStrokeWidth(void *polygonPtr, float width) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.strokeWidth = width;
}


extern int _googleMapsViewPolygonGetZIndex(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return polygon.zIndex;
}


extern void _googleMapsViewPolygonSetZIndex(void *polygonPtr, int zIndex) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.zIndex = zIndex;
}


extern bool _googleMapsViewPolygonIsVisible(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return polygon.map != nil;
}


extern void _googleMapsViewPolygonSetVisible(void *polygonPtr, void *mapPtr, bool visible) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        polygon.map = map;
    } else {
        polygon.map = nil;
    }
}


extern bool _googleMapsViewPolygonIsGeodesic(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return polygon.geodesic;
}


extern void _googleMapsViewPolygonSetGeodesic(void *polygonPtr, bool isGeodesic) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.geodesic = isGeodesic;
}


extern bool _googleMapsViewPolygonIsClickable(void *polygonPtr) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    return polygon.tappable;
}


extern void _googleMapsViewPolygonSetClickable(void *polygonPtr, bool clickable) {
    CustomGMSPolygon *polygon = (__bridge CustomGMSPolygon *) (polygonPtr);
    polygon.tappable = clickable;
}


extern void _googleMapsViewPolygonRemove(void *polygonPtr) {
    CustomGMSPolygon *polygon = (CustomGMSPolygon *) CFBridgingRelease(polygonPtr);
    polygon.map = nil;
}
}

#pragma clang diagnostic pop

