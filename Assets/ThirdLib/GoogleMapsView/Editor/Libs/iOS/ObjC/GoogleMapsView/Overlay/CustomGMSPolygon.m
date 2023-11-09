//
// Created by Taras Leskiv on 1/5/18.
//

#import "CustomGMSPolygon.h"


@implementation CustomGMSPolygon {

}
- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)polygonWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end