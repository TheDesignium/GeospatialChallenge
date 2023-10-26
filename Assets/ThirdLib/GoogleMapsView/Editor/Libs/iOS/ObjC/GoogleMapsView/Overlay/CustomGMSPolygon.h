//
// Created by Taras Leskiv on 1/5/18.
//

#import <Foundation/Foundation.h>
#import <GoogleMaps/GoogleMaps.h>


@interface CustomGMSPolygon : GMSPolygon

@property(strong, nonatomic) NSString *identifier;

- (instancetype)initWithIdentifier:(NSString *)identifier;

+ (instancetype)polygonWithIdentifier:(NSString *)identifier;

@end