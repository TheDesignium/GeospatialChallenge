#import "CustomGMSTileOverlay.h"


@implementation CustomGMSTileOverlay {

}

- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)tileLayerWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end