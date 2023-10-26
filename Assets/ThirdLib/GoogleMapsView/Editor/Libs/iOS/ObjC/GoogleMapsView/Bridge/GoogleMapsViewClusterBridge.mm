#import <GoogleMaps/GoogleMaps.h>
#import "GMUClusterManager.h"
#import "GMUDefaultClusterRenderer.h"
#import "GMUDefaultClusterIconGenerator.h"
#import "GMUNonHierarchicalDistanceBasedAlgorithm.h"
#import "GoogleMapsViewClusterItem.h"
#import "GoogleMapsViewUtils.h"
#import "GoogleMapsJsonUtils.h"
#import "GoogleMapsCustomClusterRenderer.h"
#import "CustomClusterRendererDelegate.h"

#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

static NSMutableDictionary* clusterManageImgCache = [NSMutableDictionary dictionary];

extern "C" {
void *_createGoogleMapsViewClusterManager(void *mapPtr) {
    GMSMapView *mapView = (__bridge GMSMapView *) (mapPtr);
    id <GMUClusterAlgorithm> algorithm = [[GMUNonHierarchicalDistanceBasedAlgorithm alloc] init];
    id <GMUClusterIconGenerator> iconGenerator = [[GMUDefaultClusterIconGenerator alloc] init];
    GoogleMapsCustomClusterRenderer* renderer = [[GoogleMapsCustomClusterRenderer alloc] initWithMapView:mapView
                                                                     clusterIconGenerator:iconGenerator];
    renderer.delegate = [CustomClusterRendererDelegate new];

    GMUClusterManager *clusterManager = [[GMUClusterManager alloc] initWithMap:mapView
                                                                     algorithm:algorithm
                                                                      renderer:renderer];
    [clusterManager cluster];
    return (void *) CFBridgingRetain(clusterManager);
}

void _googleMapsViewClusterManagerAddItems(void *managerPtr, char *items) {
    GMUClusterManager *clusterManager = (__bridge GMUClusterManager *) (managerPtr);
    NSString *itemsStr = [GoogleMapsViewUtils createNSStringFrom:items];

    NSArray<GoogleMapsViewClusterItem *> *clusterItems = [GoogleMapsJsonUtils deserializeClusterItems:itemsStr];
    [clusterManager addItems:clusterItems];
    [clusterManager cluster];
}

void _googleMapsViewClusterManagerAddSingleItem(void *managerPtr, char *item) {
    GMUClusterManager *clusterManager = (__bridge GMUClusterManager *) (managerPtr);
    NSString *itemStr = [GoogleMapsViewUtils createNSStringFrom:item];
    NSDictionary *itemDic = [GoogleMapsJsonUtils deserializeDictionary:itemStr];

    id <GMUClusterItem> clusterItem = [GoogleMapsJsonUtils deserializeClusterItem:itemDic withCache:nil];

    [clusterManager addItem:clusterItem];
    [clusterManager cluster];
}


void _googleMapsViewClusterManagerClearItems(void *managerPtr) {
    GMUClusterManager *clusterManager = (__bridge GMUClusterManager *) (managerPtr);
    [clusterManager clearItems];
}
}

#pragma clang diagnostic pop
