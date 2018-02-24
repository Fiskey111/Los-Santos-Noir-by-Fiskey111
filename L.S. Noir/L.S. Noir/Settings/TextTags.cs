using System.IO;
using System.Xml.Serialization;

namespace LSNoir.Settings
{
    public struct KeyVal
    {
        public string Key;
        public string Value;
        //public string GetTag => $"{{{Key}}}";
    }

    public class TextTags
    {
        [XmlIgnore]
        public static TextTags Instance
        {
            get
            {
                if (instance != null) return instance;
                else
                {
                    if (!File.Exists(Paths.PATH_TEXT_TAGS)) DataAccess.DataProvider.Instance.Save(Paths.PATH_TEXT_TAGS, new TextTags());
                    instance = DataAccess.DataProvider.Instance.Load<TextTags>(Paths.PATH_TEXT_TAGS);
                    return instance;
                }
            }
        }

        [XmlIgnore]
        private static TextTags instance;

        public KeyVal[] Tags = new[]
        {
            new KeyVal
            {
                Key = "PlayerFirstName",
                Value = "Frank",
            },
            new KeyVal
            {
                Key = "PlayerLastName",
                Value= "Pembleton",
            },
            new KeyVal
            {
                Key = "PlayerRankFull",
                Value= "Detective",
            },
            new KeyVal
            {
                Key = "PlayerRankAbbbrev",
                Value= "Det.",
            },
            new KeyVal
            {
                Key = "PlayerDivision",
                Value= "Homicide Squad",
            },
            new KeyVal
            {
                Key = "PlayerDeptFull",
                Value= "Los Santos Police Deptartment",
            },
            new KeyVal
            {
                Key = "PlayerDeptAbbrev",
                Value= "LSPD",
            },
        };
    }
}
