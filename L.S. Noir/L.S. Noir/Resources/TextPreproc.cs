using LSNoir.Settings;
using System.Collections.Generic;

namespace LSNoir.Resources
{
    static class Test
    {
        private static KeyVal[] tags;
        static Test()
        {
            tags = TextTags.Instance.Tags;
        }

        public static string RevalString(this string s)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                s = s.Replace("{" + tags[i].Key + "}", tags[i].Value);
            }
            return s;
        }

        public static void RevalStrings(this IList<string> e)
        {
            for (int i = 0; i < e.Count; i++)
            {
                e[i] = e[i].RevalString();
            }
        }
    }
}
