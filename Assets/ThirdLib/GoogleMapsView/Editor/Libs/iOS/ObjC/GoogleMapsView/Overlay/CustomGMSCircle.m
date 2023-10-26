//
// Created by Taras Leskiv on 1/1/18.
//

#import "CustomGMSCircle.h"


@implementation CustomGMSCircle {

}

- (instancetype)initWithIdentifier:(NSString *)identifier {
    self = [super init];
    if (self) {
        self.identifier = identifier;
    }

    return self;
}

+ (instancetype)circleWithIdentifier:(NSString *)identifier {
    return [[self alloc] initWithIdentifier:identifier];
}

@end