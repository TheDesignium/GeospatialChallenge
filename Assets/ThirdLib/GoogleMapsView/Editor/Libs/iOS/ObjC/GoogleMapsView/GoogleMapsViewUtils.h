#import <Foundation/Foundation.h>
#import <UIKit/UIImage.h>

@interface GoogleMapsViewUtils : NSObject

+ (NSMutableDictionary *)cache;

+ (NSString *)createNSStringFrom:(const char *)cstring;

+ (char *)createCStringFrom:(NSString *)string;

+ (NSArray *)createNSArray:(int)count values:(const char **)values;

+ (char *)cStringCopy:(const char *)string;

+ (UIImage *)createImageFromByteArray:(const void *)data dataLength:(const unsigned long)length;

+ (UIColor *)colorFromFloatArr:(float[])c;

+ (NSArray<NSNumber *> *)numberArrayFromFloatArr:(float[])numbers arrayLenght:(int)length;

+ (CGRect)scaledRectWith:(int)x y:(int)y width:(int)width height:(int)height;

+ (CGLineCap)getLineCap:(int)cap;

+ (CGLineJoin)getLineJoin:(int)join;

+ (NSArray<NSNumber *> *)createDashPatternFrom:(float *)pattern withLength:(int)length;
@end
