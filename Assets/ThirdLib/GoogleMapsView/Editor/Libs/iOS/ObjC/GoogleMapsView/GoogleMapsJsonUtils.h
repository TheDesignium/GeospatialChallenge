#import <Foundation/Foundation.h>

@class GoogleMapsViewClusterItem;
@protocol GMUClusterItem;
@class CustomGMSMarker;

@interface GoogleMapsJsonUtils : NSObject

+ (NSDictionary *)deserializeDictionary:(NSString *)jsonDic;

+ (NSArray *)deserializeArray:(NSString *)jsonArray;

+ (NSString *)serializeDictionary:(NSDictionary *)dictionary;

+ (GMSCameraPosition *)deserializeCameraFrom:(NSDictionary *)dictionary;

+ (GMSCoordinateBounds *)deserializeBounds:(NSDictionary *)boundsDic;

+ (NSString *)serializeLocation:(CLLocation *)location;

+ (UIColor *)deserializeColor:(NSDictionary *)dictionary;

+ (NSArray<UIColor *> *)deserializeColorArray:(NSArray *)colors;

+ (NSString *)serializeCoords:(CLLocationCoordinate2D)coordinate2D;

+ (CLLocationCoordinate2D)coordFromDictionary:(NSDictionary *)positionDic;

+ (NSString *)serializeColor:(UIColor *)color;

+ (CLLocationCoordinate2D)deserializeCoord:(NSDictionary *)dictionary;

+ (GMSPath *)deserializePointsArr:(NSArray *)points;

+ (NSString *)serializePoints:(GMSPath *)path;

+ (NSString *)serializeArray:(NSMutableArray *)array;

+ (NSArray<GMSPath *> *)deserializeHoles:(id)o;

+ (NSString *)serializeHoles:(NSArray<GMSPath *> *)array;

+ (NSString *)serializeBounds:(GMSCoordinateBounds *)bounds;

+ (NSString *)serializeCameraPosition:(GMSCameraPosition *)position;

+ (NSArray<NSNumber *> *)deserializeNumbersArray:(NSArray *)numbers;

+ (CustomGMSMarker *)deserializeMarkerOptions:(NSDictionary *)markerOptionsDic
                                    withCache:(NSMutableDictionary *)cache;

+ (void)setIconWithCache:(NSMutableDictionary *)cache marker:(CustomGMSMarker *)marker markerOptionsDic:(NSDictionary *)markerOptionsDic;

+ (NSArray<GoogleMapsViewClusterItem *> *)deserializeClusterItems:(NSString *)json;

+ (id <GMUClusterItem>)deserializeClusterItem:(NSDictionary *)dic withCache:(NSMutableDictionary *)cache;
@end
