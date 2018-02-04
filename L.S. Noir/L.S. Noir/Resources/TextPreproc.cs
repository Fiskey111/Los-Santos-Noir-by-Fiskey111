using LSNoir.Settings;
using System;
using System.Collections.Generic;

namespace LSNoir.Resources
{
    //name
    //dept.
    //rank
    //division

    //class TextPreproc
    //{
    //    //static ctor which loads all keyvals?
    //    // change all vals on load?
    //    private KeyVal[] tags;

    //    public TextPreproc(KeyVal[] keys)
    //    {
    //        tags = keys;
    //    }

    //    private string Get(string s)
    //    {
    //        for (int i = 0; i < tags.Length; i++)
    //        {
    //            s = s.Replace(tags[i].Key, tags[i].Value);
    //        }
    //        return s;
    //    }
    //}
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

    //class Text
    //{
    //    private string text;
    //    public Text(string s)
    //    {
    //        text = s; //replace all tags here
    //    }
    //    public static implicit operator string(Text t) => t.text;
    //    public static implicit operator Text(string s) => new Text(s);
    //}
}
