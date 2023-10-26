#import <GoogleMaps/GoogleMaps.h>
#import "CustomClusterRendererDelegate.h"
#import "GoogleMapsViewClusterItem.h"
#import "CustomGMSMarker.h"


@implementation CustomClusterRendererDelegate {

}
- (void)renderer:(id <GMUClusterRenderer>)renderer willRenderMarker:(GMSMarker *)marker {
    NSString *className = GoogleMapsViewClusterItem.class.description;
    if ([[[marker.userData class] description] isEqualToString:className]) {
        GoogleMapsViewClusterItem *item = marker.userData;
        marker.title = item.marker.title;
        marker.snippet = item.marker.snippet;
        marker.rotation = item.marker.rotation;
        marker.icon = item.marker.icon;
        marker.zIndex = item.marker.zIndex;
        marker.groundAnchor = item.marker.groundAnchor;
        marker.infoWindowAnchor = item.marker.infoWindowAnchor;
        marker.draggable = item.marker.draggable;
        marker.flat = item.marker.flat;
        marker.opacity = item.marker.opacity;
    }
}

@end