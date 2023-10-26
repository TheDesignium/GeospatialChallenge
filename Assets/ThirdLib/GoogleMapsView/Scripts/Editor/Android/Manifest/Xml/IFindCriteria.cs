namespace GetSocialSdk.Editor.Android.Manifest.GoogleMaps
{
    public interface IFindCriteria<in T>
    {
        bool SatisfiesCriteria(T obj);
    }
}