#import "GoogleMapsViewDelegate.h"
#import "CustomGMSMarker.h"
#import "CustomGMSCircle.h"
#import "CustomGMSPolyline.h"
#import "CustomGMSPolygon.h"
#import "CustomGMSGroundOverlay.h"

@implementation GoogleMapsViewDelegate {
    
}

+ (GoogleMapsViewDelegate *)instance {
    static GoogleMapsViewDelegate *_instance = nil;
    
    @synchronized (self) {
        if (_instance == nil) {
            _instance = [[self alloc] init];
        }
    }
    
    return _instance;
}

- (void)mapView:(GMSMapView *)mapView didTapAtCoordinate:(CLLocationCoordinate2D)coordinate {
    if (_mapTapped) {
        _mapTapped(coordinate);
    }
}

- (void)mapView:(GMSMapView *)mapView didLongPressAtCoordinate:(CLLocationCoordinate2D)coordinate {
    if (_mapLongTapped) {
        _mapLongTapped(coordinate);
    }
}

- (BOOL)mapView:(GMSMapView *)mapView didTapMarker:(GMSMarker *)marker {
    if (_markerTapped) {
        return _markerTapped((CustomGMSMarker *) marker);
    }
    
    return NO;
}

- (void)mapView:(GMSMapView *)mapView didBeginDraggingMarker:(GMSMarker *)marker {
    if (_markerBeginDrag) {
        _markerBeginDrag((CustomGMSMarker *)marker);
    }
}

-(void)mapView:(GMSMapView *)mapView didDragMarker:(GMSMarker *)marker {
    if (_markerDrag) {
        _markerDrag((CustomGMSMarker *)marker);
    }
}

-(void)mapView:(GMSMapView *)mapView didEndDraggingMarker:(GMSMarker *)marker {
    if (_markerEndDrag) {
        _markerEndDrag((CustomGMSMarker *)marker);
    }
}

- (void)mapView:(GMSMapView *)mapView didTapInfoWindowOfMarker:(GMSMarker *)marker {
    if (_markerInfoWindowTapped) {
        _markerInfoWindowTapped((CustomGMSMarker *) marker);
    }
}

- (void)mapView:(GMSMapView *)mapView didTapOverlay:(GMSOverlay *)overlay {
    if ([overlay isKindOfClass:CustomGMSCircle.class]) {
        if (_circleTapped) {
            _circleTapped((CustomGMSCircle *) overlay);
        }
    } else if ([overlay isKindOfClass:CustomGMSPolyline.class]) {
        if (_polylineTapped) {
            _polylineTapped((CustomGMSPolyline *) overlay);
        }
    } else if ([overlay isKindOfClass:CustomGMSPolygon.class]) {
        if (_polygonTapped) {
            _polygonTapped((CustomGMSPolygon *) overlay);
        }
    } else if ([overlay isKindOfClass:CustomGMSGroundOverlay.class]) {
        if (_groundOverlayTapped) {
            _groundOverlayTapped((CustomGMSGroundOverlay *) overlay);
        }
    }
}

- (void)mapView:(GMSMapView *)mapView willMove:(BOOL)gesture {
    if (_cameraStartedMoving) {
        int reasonGesture = 1;
        int developerAnimation = 3;
        _cameraStartedMoving(gesture ? reasonGesture : developerAnimation);
    }
}

-(void)mapView:(GMSMapView *)mapView idleAtCameraPosition:(GMSCameraPosition *)position {
    if (_cameraIdle) {
        _cameraIdle();
    }
}

@end
