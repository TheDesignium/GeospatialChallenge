#import <Foundation/Foundation.h>

#import <GoogleMaps/GoogleMaps.h>
#import "GMUClusterItem.h"

@class CustomGMSMarker;

@interface GoogleMapsViewClusterItem : NSObject <GMUClusterItem>

@property(nonatomic, readonly) CustomGMSMarker *marker;

- (instancetype)initWithMarker:(CustomGMSMarker *)marker;

+ (instancetype)itemWithMarker:(CustomGMSMarker *)marker;

@end
