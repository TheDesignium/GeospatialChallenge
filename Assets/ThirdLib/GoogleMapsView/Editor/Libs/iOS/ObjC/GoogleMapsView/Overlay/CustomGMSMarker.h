#import <Foundation/Foundation.h>
#import <GoogleMaps/GoogleMaps.h>

@interface CustomGMSMarker : GMSMarker

@property(strong, nonatomic) NSString *identifier;

- (instancetype)initWithIdentifier:(NSString *)identifier;

+ (instancetype)markerWithIdentifier:(NSString *)identifier;

@end