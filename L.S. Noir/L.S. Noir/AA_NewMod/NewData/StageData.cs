using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace LSNoir.Data.NewData
{
    public class StageData : IData
    {
        // IData
        public string ID { get; set; }
        public string Name { get; set; }

        // String
        public string SceneID { get; set; }

        // Int
        public int DelayMinSeconds { get; set; }
        public int DelayMaxSeconds { get; set; }

        // Blip Data
        public CallBlipData BlipData { get; set; }

        // Notification Data
        public CallNotificationData NotificationData { get; set; }

        // Data IDs
        public string[] ID_EntityData { get; set; }
        public string[] ID_CSIData { get; set; }
        public string[] ID_WrittenData { get; set; }
        public string[] ID_SceneData { get; set; }
        public string[] ID_InterrogationData { get; set; }

        // List<string>
        public List<string> NextScript { get; set; }
    }

    public class StageBlipData
    {
        public string BlipName { get; set; }
        public float BlipRadius { get; set; }
        public int AreaRadius { get; set; }
        public Vector3 Position { get; set; }
        public Color BlipColor { get; set; }
        public BlipSprite BlipSprite { get; set; }

        public Blip Create()
        {
            return new Blip(Position, BlipRadius)
            {
                Color = BlipColor,
                Sprite = BlipSprite,
                Name = BlipName
            };
        }
    }

    public class CallNotificationData
    {
        public string TextureDictionary { get; set; }
        public string TextureName { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }

        public uint Show()
        {
            if (string.IsNullOrWhiteSpace(TextureDictionary)) TextureDictionary = "3dtextures";
            if (string.IsNullOrWhiteSpace(TextureName)) TextureName = "mpgroundlogo_cops";
            return Game.DisplayNotification(TextureDictionary, TextureName, Title, Subtitle, Text);
        }
    }
}
