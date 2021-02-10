using System.Collections.Generic;
using System.Linq;

namespace YouTubeStreamTemplatesCrossPlatform
{
    public static class ExtensionMethods
    {
        public static KeyValuePair<string, string> FirstMatching(this Dictionary<string, string> categories,
                                                                 string category)
        {
            return categories.First(kp => kp.Key.Equals(category));
        }
    }
}