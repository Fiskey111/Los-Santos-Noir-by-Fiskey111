using System;

namespace LSNoir.Resources
{
    //name
    //dept.
    //rank
    //division

    struct KeyVal
    {
        public string Key;
        public string Value;
    }

    class TextPreproc
    {
        //static ctor which loads all keyvals?
        // change all vals on load?
        private KeyVal[] tags;

        public TextPreproc(KeyVal[] keys)
        {
            tags = keys;
        }

        private string Get(string s)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                s = s.Replace(tags[i].Key, tags[i].Value);
            }
            return s;
        }
    }
    static class Test
    {
        private static KeyVal[] tags;
        static Test()
        {
            tags = new KeyVal[] { }; //load from file?
        }

        public static string Reval(this string s)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                s = s.Replace(tags[i].Key, tags[i].Value);
            }
            return s;
        }
    }

    class Text
    {
        private string text;
        public Text(string s)
        {
            text = s; //replace all tags here
        }
        public static implicit operator string(Text t) => t.text;
        public static implicit operator Text(string s) => new Text(s);
    }
}
