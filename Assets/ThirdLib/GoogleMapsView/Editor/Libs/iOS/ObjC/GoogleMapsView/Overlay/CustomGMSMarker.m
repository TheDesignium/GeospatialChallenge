//
// Created by Taras Leskiv on 1/5/18.
//

#import "CustomGMSMarker.h"


@implementation CustomGMSMarker {

}
- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)markerWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end