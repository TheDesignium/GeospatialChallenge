using System.Collections.Generic;

namespace GetSocialSdk.Editor.Android.Manifest.GoogleMaps
{
    public class IntentFilter : AndroidManifestNode
    {
        public IntentFilter(bool autoVerify) : base(
            IntentFilterTag, 
            attributes: new Dictionary<string, string>()
            {
                {"android:autoVerify", autoVerify.ToString().ToLower()}
            })
        {
        }
    }
}