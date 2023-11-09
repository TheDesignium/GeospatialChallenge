//
// Created by Taras Leskiv on 1/5/18.
//

#import "CustomGMSGroundOverlay.h"


@implementation CustomGMSGroundOverlay {

}
- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)overlayWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end