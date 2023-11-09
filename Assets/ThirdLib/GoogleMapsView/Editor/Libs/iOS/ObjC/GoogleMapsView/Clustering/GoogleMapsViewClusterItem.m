#import "GoogleMapsViewClusterItem.h"
#import "CustomGMSMarker.h"

@implementation GoogleMapsViewClusterItem {

}

- (CLLocationCoordinate2D)position {
    return _marker.position;
}


- (instancetype)initWithMarker:(CustomGMSMarker *)marker {
    self = [super init];
    if (self) {
        _marker = marker;
    }

    return self;
}

+ (instancetype)itemWithMarker:(CustomGMSMarker *)marker {
    return [[self alloc] initWithMarker:marker];
}

@end