using System.Collections.Generic;

namespace GetSocialSdk.Editor.Android.Manifest.GoogleMaps
{
	public class UsesApacheHttpLibrary : AndroidManifestNode
	{
		public UsesApacheHttpLibrary() : base(
			"uses-library",
			ApplicationTag,
			new Dictionary<string, string> {{NameAttribute, "org.apache.http.legacy"}, {RequiredAttribute, "false"}})
		{
		}

		public override string ToString()
		{
			return string.Format("Meta data {0}", Attributes[NameAttribute]);
		}
	}
}