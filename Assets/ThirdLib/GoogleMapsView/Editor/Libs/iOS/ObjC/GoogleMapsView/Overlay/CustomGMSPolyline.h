#import <Foundation/Foundation.h>
#import <GoogleMaps/GoogleMaps.h>


@interface CustomGMSPolyline : GMSPolyline

@property(strong, nonatomic) NSString *identifier;

- (instancetype)initWithIdentifier:(NSString *)identifier;

+ (instancetype)polylineWithIdentifier:(NSString *)identifier;

@end