using UnityEngine;
using System.Collections.Generic;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PermissionManager : MonoBehaviour
{
    // Define a struct to hold permission information
    [System.Serializable]
    public struct PermissionItem
    {
        public string name;
        public bool isRequired;
    }

    // List of common Android permissions
    // You need to add or remove permissions as per your requirements
    public PermissionItem[] permissions = new PermissionItem[]
    {
        new PermissionItem { name = "android.permission.CAMERA", isRequired = false },
        new PermissionItem { name = "android.permission.ACCESS_FINE_LOCATION", isRequired = false },
        new PermissionItem { name = "android.permission.ACCESS_COARSE_LOCATION", isRequired = false },
        new PermissionItem { name = "android.permission.READ_EXTERNAL_STORAGE", isRequired = false },
        new PermissionItem { name = "android.permission.WRITE_EXTERNAL_STORAGE", isRequired = false },
        // Add more permissions as needed
    };

    void Start()
    {
        #if PLATFORM_ANDROID
        RequestPermissions();
        #endif
    }

    #if PLATFORM_ANDROID
    void RequestPermissions()
    {
        foreach (var permission in permissions)
        {
            if (permission.isRequired && !Permission.HasUserAuthorizedPermission(permission.name))
            {
                Permission.RequestUserPermission(permission.name);
            }
        }
    }
    #endif
}
