//
// Created by Taras Leskiv on 1/5/18.
//

#import "CustomGMSPolyline.h"


@implementation CustomGMSPolyline {

}
- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)polylineWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end