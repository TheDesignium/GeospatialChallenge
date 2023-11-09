#import "GoogleMapsViewUtils.h"

@implementation GoogleMapsViewUtils


+ (NSMutableDictionary *)cache {
    static NSMutableDictionary *_cache = nil;
    
    @synchronized (self) {
        if (_cache == nil) {
            _cache = [NSMutableDictionary dictionary];;
        }
    }
    
    return _cache;
}

// Converts C style string to NSString
+ (NSString *)createNSStringFrom:(const char *)cstring {
    return [NSString stringWithUTF8String:(cstring ?: "")];
}

+ (NSArray *)createNSArray:(int)count values:(const char **)values {
    if (count == 0) {
        return nil;
    }

    NSMutableArray *mutableArray = [NSMutableArray array];

    for (NSUInteger i = 0; i < count; i++) {
        mutableArray[i] = [self createNSStringFrom:values[i]];
    }

    return mutableArray;
}

+ (char *)cStringCopy:(const char *)string {
    char *res = (char *) malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

+ (char *)createCStringFrom:(NSString *)string {
    if (!string) {
        string = @"";
    }
    return [self cStringCopy:[string UTF8String]];
}

+ (UIImage *)createImageFromByteArray:(const void *)data dataLength:(const unsigned long)length {
    NSData *imageData = [[NSData alloc] initWithBytes:data length:length];
    UIImage *image = [UIImage imageWithData:imageData];
    return image;
}

+ (UIColor *)colorFromFloatArr:(float[])c {
    return [UIColor colorWithRed:c[0] green:c[1] blue:c[2] alpha:c[3]];
}

+ (NSArray<NSNumber *> *)numberArrayFromFloatArr:(float[])numbers arrayLenght:(int)length {
    NSMutableArray<NSNumber *> *result = [[NSMutableArray alloc] init];
    for (int i = 0; i < length; i++) {
        NSNumber *number = [[NSNumber alloc] initWithFloat:numbers[i]];
        [result addObject:number];
    }
    return result;
}

+ (CGRect)scaledRectWith:(int)x
                       y:(int)y
                   width:(int)width
                  height:(int)height {
    CGFloat scale = [[UIScreen mainScreen] nativeScale];
    return CGRectMake(x / scale, y / scale, width / scale, height / scale);
}

+ (CGLineCap)getLineCap:(int)cap {
    if (cap == 0) {
        return kCGLineCapButt;
    } else if (cap == 1) {
        return kCGLineCapRound;
    } else if (cap == 2) {
        return kCGLineCapSquare;
    } else {
        return kCGLineCapRound;
    }
}

+ (CGLineJoin)getLineJoin:(int)join {
    if (join == 0) {
        return kCGLineJoinMiter;
    } else if (join == 1) {
        return kCGLineJoinRound;
    } else if (join == 2) {
        return kCGLineJoinBevel;
    } else {
        return kCGLineJoinRound;
    }
}


+ (NSArray<NSNumber *> *)createDashPatternFrom:(float *)pattern withLength:(int)length {
    NSMutableArray<NSNumber *> *patternArray = [NSMutableArray new];
    for (int i = 0; i < length; ++i) {
        [patternArray addObject:@(pattern[i])];
    }
    return patternArray;
}
@end
