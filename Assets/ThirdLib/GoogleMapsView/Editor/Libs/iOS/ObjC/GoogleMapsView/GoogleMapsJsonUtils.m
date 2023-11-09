#import <GoogleMaps/GoogleMaps.h>
#import "GoogleMapsJsonUtils.h"
#import "GoogleMapsViewClusterItem.h"
#import "GMUClusterItem.h"
#import "CustomGMSMarker.h"

@implementation GoogleMapsJsonUtils

+ (NSDictionary *)deserializeDictionary:(NSString *)jsonDic {
    NSError *e = nil;
    NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:[jsonDic dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&e];
    if (dictionary != nil) {
        NSMutableDictionary *prunedDict = [NSMutableDictionary dictionary];
        [dictionary enumerateKeysAndObjectsUsingBlock:^(NSString *key, id obj, BOOL *stop) {
            if (![obj isKindOfClass:[NSNull class]]) {
                prunedDict[key] = obj;
            }
        }];
        return prunedDict;
    }
    return dictionary;
}

+ (NSArray *)deserializeArray:(NSString *)jsonArray {
    NSError *e = nil;
    NSArray *array = [NSJSONSerialization JSONObjectWithData:[jsonArray dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&e];
    if (array != nil) {
        NSMutableArray *prunedArr = [NSMutableArray array];
        [array enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
            if (![obj isKindOfClass:[NSNull class]]) {
                prunedArr[idx] = obj;
            }
        }];
        return prunedArr;
    }
    return array;
}

+ (NSString *)serializeArray:(NSArray *)array {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:array
                                                       options:nil
                                                         error:&error];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSString *)serializeDictionary:(NSDictionary *)dictionary {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary
                                                       options:nil
                                                         error:&error];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (GMSCameraPosition *)deserializeCameraFrom:(NSDictionary *)dictionary {
    NSString *key = @"camera";
    if (dictionary[key]) {
        NSDictionary *cameraDic = dictionary[key];
        CLLocationDegrees latitude = [cameraDic[@"cameraPositionLat"] doubleValue];
        CLLocationDegrees longitude = [cameraDic[@"cameraPositionLng"] doubleValue];
        float zoom = [cameraDic[@"cameraPositionZoom"] floatValue];
        float bearing = [cameraDic[@"cameraPositionBearing"] floatValue];
        float tilt = [cameraDic[@"cameraPositionTilt"] floatValue];

        return [GMSCameraPosition cameraWithLatitude:latitude longitude:longitude zoom:zoom bearing:bearing viewingAngle:tilt];
    }
    return nil;
}

+ (GMSCoordinateBounds *)deserializeBounds:(NSDictionary *)boundsDic {
    if (boundsDic == nil) {
        return nil;
    }

    CLLocationDegrees southWestLat = [boundsDic[@"latLngBoundsSouthWestLat"] doubleValue];
    CLLocationDegrees southWestLng = [boundsDic[@"latLngBoundsSouthWestLng"] doubleValue];
    CLLocationDegrees northEastLat = [boundsDic[@"latLngBoundsNorthEastLat"] doubleValue];
    CLLocationDegrees northEastLng = [boundsDic[@"latLngBoundsNorthEastLng"] doubleValue];
    CLLocationCoordinate2D NE = CLLocationCoordinate2DMake(northEastLat, northEastLng);
    CLLocationCoordinate2D SW = CLLocationCoordinate2DMake(southWestLat, southWestLng);
    return [[GMSCoordinateBounds alloc] initWithCoordinate:NE coordinate:SW];
}

+ (NSString *)serializeLocation:(CLLocation *)location {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"lat"] = @(location.coordinate.latitude);
    dictionary[@"lng"] = @(location.coordinate.longitude);
    dictionary[@"timestamp"] = @(floor([location.timestamp timeIntervalSince1970]));
    dictionary[@"course"] = @(location.course);
    dictionary[@"altitude"] = @(location.altitude);
    dictionary[@"horizontalAccuracy"] = @(location.horizontalAccuracy);
    dictionary[@"verticalAccuracy"] = @(location.verticalAccuracy);
    dictionary[@"speed"] = @(location.speed);
    return [self serializeDictionary:dictionary];
}

+ (UIColor *)deserializeColor:(NSDictionary *)dictionary {
    CGFloat red = [dictionary[@"r"] floatValue];
    CGFloat green = [dictionary[@"g"] floatValue];
    CGFloat blue = [dictionary[@"b"] floatValue];
    CGFloat alpha = [dictionary[@"a"] floatValue];
    return [UIColor colorWithRed:red green:green blue:blue alpha:alpha];
}

+ (NSArray<UIColor *> *)deserializeColorArray:(NSArray *)colors {
    NSMutableArray<UIColor *> *result = [NSMutableArray new];

    for (NSDictionary *dic in colors) {
        UIColor *c = [self deserializeColor:dic];
        [result addObject:c];
    }

    return result;
}

+ (NSString *)serializeCoords:(CLLocationCoordinate2D)coordinate2D {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"lat"] = @(coordinate2D.latitude);
    dictionary[@"lng"] = @(coordinate2D.longitude);
    return [self serializeDictionary:dictionary];
}

+ (CLLocationCoordinate2D)coordFromDictionary:(NSDictionary *)positionDic {
    CLLocationDegrees latitude = [positionDic[@"lat"] doubleValue];
    CLLocationDegrees longitude = [positionDic[@"lng"] doubleValue];
    CLLocationCoordinate2D position = CLLocationCoordinate2DMake(latitude, longitude);
    return position;
}

+ (NSString *)serializeColor:(UIColor *)color {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    CGFloat red, green, blue, alpha;
    [color getRed:&red green:&green blue:&blue alpha:&alpha];
    dictionary[@"r"] = @(red);
    dictionary[@"g"] = @(green);
    dictionary[@"b"] = @(blue);
    dictionary[@"a"] = @(alpha);
    return [self serializeDictionary:dictionary];
}

+ (CLLocationCoordinate2D)deserializeCoord:(NSDictionary *)dictionary {
    CLLocationCoordinate2D result;
    result.latitude = [dictionary[@"lat"] doubleValue];
    result.longitude = [dictionary[@"lng"] doubleValue];
    return result;
}

+ (GMSPath *)deserializePointsArr:(NSArray *)points {
    GMSMutablePath *path = [GMSMutablePath path];
    for (NSDictionary *dic in points) {
        CLLocationCoordinate2D coord = [GoogleMapsJsonUtils deserializeCoord:dic];
        [path addCoordinate:coord];
    }
    return path;
}

+ (NSString *)serializePoints:(GMSPath *)path {
    NSMutableArray *points = [NSMutableArray arrayWithCapacity:path.count];
    for (NSUInteger i = 0; i < path.count; ++i) {
        CLLocationCoordinate2D coord = [path coordinateAtIndex:i];
        NSMutableDictionary *coordsDic = [NSMutableDictionary dictionary];
        coordsDic[@"lat"] = @(coord.latitude);
        coordsDic[@"lng"] = @(coord.longitude);
        [points addObject:coordsDic];
    }
    return [GoogleMapsJsonUtils serializeArray:points];
}

+ (NSArray<GMSPath *> *)deserializeHoles:(NSArray *)holesArr {
    NSMutableArray <GMSPath *> *holes = [NSMutableArray new];
    for (NSArray *hole in holesArr) {
        GMSMutablePath *path = [GMSMutablePath path];

        for (NSArray *coord in hole) {
            CLLocationCoordinate2D holeCoord = [self deserializeCoord:coord];
            [path addCoordinate:holeCoord];
        }

        [holes addObject:path];
    }

    return holes;
}

+ (NSString *)serializeHoles:(NSArray<GMSPath *> *)holes {
    NSMutableArray *resultHoles = [NSMutableArray arrayWithCapacity:holes.count];

    for (GMSPath *path in holes) {
        NSMutableArray *coordsOfHole = [NSMutableArray new];
        for (NSUInteger i = 0; i < path.count; ++i) {
            CLLocationCoordinate2D coord = [path coordinateAtIndex:i];
            NSMutableDictionary *coordsDic = [NSMutableDictionary dictionary];
            coordsDic[@"lat"] = @(coord.latitude);
            coordsDic[@"lng"] = @(coord.longitude);
            [coordsOfHole addObject:coordsDic];
        }
        [resultHoles addObject:coordsOfHole];
    }

    return [GoogleMapsJsonUtils serializeArray:resultHoles];
}

+ (NSString *)serializeBounds:(GMSCoordinateBounds *)bounds {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"latLngBoundsSouthWestLat"] = @(bounds.southWest.latitude);
    dictionary[@"latLngBoundsSouthWestLng"] = @(bounds.southWest.longitude);
    dictionary[@"latLngBoundsNorthEastLat"] = @(bounds.northEast.latitude);
    dictionary[@"latLngBoundsNorthEastLng"] = @(bounds.northEast.longitude);
    return [self serializeDictionary:dictionary];
}

+ (NSString *)serializeCameraPosition:(GMSCameraPosition *)position {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"lat"] = @(position.target.latitude);
    dictionary[@"lng"] = @(position.target.longitude);
    dictionary[@"zoom"] = @(position.zoom);
    dictionary[@"tilt"] = @(position.viewingAngle);
    dictionary[@"bearing"] = @(position.bearing);
    return [self serializeDictionary:dictionary];
}

+ (NSArray<NSNumber *> *)deserializeNumbersArray:(NSArray *)numbers {
    NSMutableArray<NSNumber *> *result = [NSMutableArray new];

    for (NSNumber *n in numbers) {
        [result addObject:n];
    }

    return result;
}

+ (void)setIconWithCache:(NSMutableDictionary *)cache marker:(CustomGMSMarker *)marker markerOptionsDic:(NSDictionary *)markerOptionsDic {
    NSString *iconKey = @"icon";
    if (markerOptionsDic[iconKey]) {
        NSString *iconAssetName = markerOptionsDic[@"iconAssetName"];
        if (cache[iconAssetName]) {
            marker.icon = cache[iconAssetName];
        } else {
            NSData *imageData = [NSData dataWithContentsOfFile:markerOptionsDic[iconKey]];
            UIImage *markerImage = [UIImage imageWithData:imageData scale:[UIScreen mainScreen].scale];
            cache[iconAssetName] = markerImage;
            marker.icon = markerImage;
        }
    } else if (markerOptionsDic[@"iconHue"]) {
        int hue = [markerOptionsDic[@"iconHue"] intValue];
        float hueFloat = (float) hue / 360.f;
        UIColor *color = [UIColor colorWithHue:hueFloat saturation:1.f brightness:1.f alpha:1.f];
        marker.icon = [GMSMarker markerImageWithColor:color];
    } else if (markerOptionsDic[@"iconBytes"]) {
        NSString *iconBytes = markerOptionsDic[@"iconBytes"];
        NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:iconBytes options:0];
        marker.icon = [UIImage imageWithData:decodedData scale:[UIScreen mainScreen].scale];
    }
}

+ (CustomGMSMarker *)deserializeMarkerOptions:(NSDictionary *)markerOptionsDic withCache:(NSMutableDictionary *)cache {
    CustomGMSMarker *marker = [CustomGMSMarker markerWithIdentifier:[NSUUID UUID].UUIDString];

    [self setIconWithCache:cache marker:marker markerOptionsDic:markerOptionsDic];

    marker.position = [GoogleMapsJsonUtils coordFromDictionary:markerOptionsDic[@"position"]];
    marker.zIndex = [markerOptionsDic[@"zIndex"] intValue];

    marker.groundAnchor = CGPointMake([markerOptionsDic[@"anchorU"] floatValue], [markerOptionsDic[@"anchorV"] floatValue]);
    marker.infoWindowAnchor = CGPointMake([markerOptionsDic[@"infoWindowAnchorU"] floatValue], [markerOptionsDic[@"infoWindowAnchorV"] floatValue]);
    marker.title = markerOptionsDic[@"title"];
    marker.snippet = markerOptionsDic[@"snippet"];
    marker.draggable = [markerOptionsDic[@"draggable"] boolValue];
    marker.flat = [markerOptionsDic[@"flat"] boolValue];
    marker.rotation = [markerOptionsDic[@"rotation"] floatValue];
    marker.opacity = [markerOptionsDic[@"alpha"] floatValue];
    return marker;
}


+ (NSArray<GoogleMapsViewClusterItem *> *)deserializeClusterItems:(NSString *)json {
    NSMutableArray <GoogleMapsViewClusterItem *> *result = [NSMutableArray new];

    NSArray *jsonItems = [self deserializeArray:json];
    for (NSDictionary *dic in jsonItems) {
        [result addObject:[self deserializeClusterItem:dic withCache:nil]];
    }

    return result;
}

+ (id <GMUClusterItem>)deserializeClusterItem:(NSDictionary *)dic withCache:(NSMutableDictionary *)cache {
    CustomGMSMarker *marker = [self deserializeMarkerOptions:dic withCache:cache];
    GoogleMapsViewClusterItem *clusterItem = [GoogleMapsViewClusterItem itemWithMarker:marker];
    return clusterItem;
}
@end
