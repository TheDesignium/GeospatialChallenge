typedef void(ActionVoidCallbackDelegate)(void *actionPtr);

typedef void(ActionIntCallbackDelegate)(void *actionPtr, int data);

typedef void(ActionStringCallbackDelegate)(void *actionPtr, const char *data);

typedef void(OnDateSelectedDelegate)(void *callbackPtr, int year, int month, int day, int hour, int minute);

typedef void(OnLocationSelectedDelegate)(void* callbackPtr, double lat, double lng);

typedef void(OnItemClickedDelegate)(void* callbackPtr, void* itemPtr, void* mapPtr);

typedef void(OnPinSelectedDelegate)(void* callbackPtr, double lat, double lng, const char* title, const char* subtitle);

typedef void(ImageResultDelegate)(void *callbackPtr, const void *byteArrPtr, int arrayLength);


#define SYSTEM_VERSION_EQUAL_TO(v)                  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedSame)
#define SYSTEM_VERSION_GREATER_THAN(v)              ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedDescending)
#define SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN(v)                 ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN_OR_EQUAL_TO(v)     ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedDescending)