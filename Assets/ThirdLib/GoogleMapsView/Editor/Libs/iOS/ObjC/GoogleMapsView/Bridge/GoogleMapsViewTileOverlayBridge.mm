
#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#import <GoogleMaps/GoogleMaps.h>
#import "CustomGMSTileOverlay.h"
#import "GoogleMapsViewUtils.h"

extern "C" {
extern char *_googleMapsViewTileOverlayGetId(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    return [GoogleMapsViewUtils createCStringFrom:tileOverlay.identifier];
}


extern int _googleMapsViewTileOverlayGetZIndex(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    return tileOverlay.zIndex;
}


extern void _googleMapsViewTileOverlaySetZIndex(void *overlayPtr, int zIndex) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    tileOverlay.zIndex = zIndex;
}


extern float _googleMapsViewTileOverlayGetTransparency(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    return 1.0f - tileOverlay.opacity;
}


extern void _googleMapsViewTileOverlaySetTransparency(void *overlayPtr, float transparency) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    tileOverlay.opacity = 1.0f - transparency;
}


extern bool _googleMapsViewTileOverlayIsVisible(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    return tileOverlay.map != nil;
}


extern void _googleMapsViewTileOverlaySetVisible(void *overlayPtr, void *mapPtr, bool visible) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    if (visible) {
        GMSMapView *map = (__bridge GMSMapView *) (mapPtr);
        tileOverlay.map = map;
    } else {
        tileOverlay.map = nil;
    }
}


extern void _googleMapsViewTileOverlayRemove(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    tileOverlay.map = nil;
}


extern void _googleMapsViewTileOverlayClearTileCache(void *overlayPtr) {
    CustomGMSTileOverlay *tileOverlay = (__bridge CustomGMSTileOverlay *) (overlayPtr);
    [tileOverlay clearTileCache];
}
}

#pragma clang diagnostic pop

