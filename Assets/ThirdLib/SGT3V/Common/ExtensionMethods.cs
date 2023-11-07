using UnityEngine;

namespace SGT3V.Common
{
    public static class ExtensionMethods
    {
        public static int Mod(this int value, int mod)
        {
            return (value % mod + mod) % mod;
        }

        public static void DestroyChildren(this Transform transform)
        {
            var children = new GameObject[transform.childCount];

            for (var i = 0; i < children.Length; i++)
            {
                children[i] = transform.GetChild(i).gameObject;
            }

            foreach (var child in children)
            {
                Object.DestroyImmediate(child);
            }
        }
    }
}
