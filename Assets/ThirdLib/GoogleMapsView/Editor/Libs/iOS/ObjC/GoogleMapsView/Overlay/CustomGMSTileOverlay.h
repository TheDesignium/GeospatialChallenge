#import <Foundation/Foundation.h>
#import <GoogleMaps/GoogleMaps.h>
#import "GMUHeatmapTileLayer.h"

@interface CustomGMSTileOverlay : GMUHeatmapTileLayer

@property(strong, nonatomic) NSString *identifier;

- (instancetype)initWithIdentifier:(NSString *)identifier;

+ (instancetype)tileLayerWithIdentifier:(NSString *)identifier;

@end