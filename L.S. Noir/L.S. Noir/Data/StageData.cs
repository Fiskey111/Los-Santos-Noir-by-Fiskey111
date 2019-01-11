using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace LSNoir.Data
{
    public class StageData : IIdentifiable
    {
        public string ID { get; set; }
        public string Name;
        public string StageType; //name of an actual class type
        public string Address;
        public string SceneID;

        [NonSerialized]
        public CaseData ParentCase; //TODO: replace with caseID? + GetParentCase()

        public Vector3 CallPosition;
        public float CallAreaRadius;
        public CallBlipData CallBlip;
        public NotificationData CallNotification;

        public bool PlaySoundClosingIn;

        public string MsgSuccess;
        public string MsgFailure; //used in failure screen

        public int DelayMinSeconds;
        public int DelayMaxSeconds;

        public ResourceData[] Resources;

        [XmlArrayItem("Scripts")]
        public List<List<string>> FinishPriorThis;

        [XmlArrayItem("Set")]
        public List<List<string>> NextScripts;

        public T GetResourceByName<T>(string name) where T : class, IIdentifiable
        {
            var id = Resources.FirstOrDefault(r => r.Name == name).ID;
            return ParentCase.GetResourceByID<T>(id);
        }
        //STAGE LEVEL =/= CASE LEVEL!!!
        public List<T> GetAllStageResourcesOfType<T>() where T : class, IIdentifiable
            => GetAllIDsOfType<T>().Select(i => ParentCase.GetResourceByID<T>(i)).ToList();

        public List<string> GetAllIDsOfType<T>() where T : class, IIdentifiable
            => Resources.Where(r => r.StringType == typeof(T).Name).Select(s => s.ID).ToList();

        public StageData()
        {
        }

        public class ResourceData
        {
            public string Name;
            [XmlElement(ElementName ="ResType")]
            public string StringType;
            public string ID;
        }
    }

    public class NotificationData
    {
        public string TextureDic;
        public string TextureName;
        public string Title;
        public string Subtitle;
        public string Text;

        public uint DisplayNotification() => Game.DisplayNotification(TextureDic, TextureName, Title, Subtitle, Text);
    }

    public class CallBlipData
    {
        public Vector3 Position;
        public float Radius;
        public BlipSprite Sprite;
        public string Name;
        public string Color;

        public Blip CreateBlip()
        {
            Blip b = Radius == 0 ? new Blip(Position) : new Blip(Position, Radius);
            b.Sprite = Sprite;
            b.Name = Name;
            b.Color = ColorTranslator.FromHtml(Color);
            return b;
        }
    }
}
