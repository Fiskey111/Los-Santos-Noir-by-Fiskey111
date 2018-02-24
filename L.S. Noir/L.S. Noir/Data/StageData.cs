using LSNoir.Resources;
using LSNoir.Settings;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LSNoir.Data
{
    //TODO:
    // - make fields readonly to disable changes?
    public class StageData_Old : IIdentifiable
    {
        public string ID { get; set; }

        public string Name;

        public string StageType;

        [NonSerialized]
        public CaseData ParentCase; //TODO: replace with caseID? + GetParentCase()

        public Vector3 CallPosition;

        public float CallBlipRad;
        public BlipSprite CallBlipSprite;
        public string CallBlipName;
        public string CallBlipColor;

        public bool PlaySoundClosingIn;
        public string SceneID;

        public string MsgSuccess;
        public string MsgFailure; //used in failure screen

        public int DelayMinSeconds;
        public int DelayMaxSeconds;

        //public string[] NextScripts = new string[] { };

        [XmlArrayItem("Set")]
        public List<List<string>> NextScripts;

        //NEW SHIT:
        public List<string> AccessoryData;
        public float CallAreaRadius;


  //      <ListOfThings>
  //  <ArrayOfString>
  //    <string>pos1</string>
  //    <string>pos2</string>
  //  </ArrayOfString>
  //  <ArrayOfString>
  //    <string>pos3</string>
  //    <string>pos4</string>
  //  </ArrayOfString>
  //</ListOfThings>

            
        [XmlArrayItem("Scripts")]
        public List<List<string>> FinishPriorThis;

        public string NotificationTexDic;
        public string NotificationTexName;
        public string NotificationTitle;
        public string NotificationSubtitle;
        public string NotificationText;

        public string VictimID;
        public string[] EvidenceID;
        public string[] WitnessID;
        public string OfficerID;
        public string CoronerID;
        public string EmsID;

        public string[] SuspectsID;
        public string[] PersonsID;

        public string[] DialogsID;
        public string[] ReportsID;
        public string[] NotesID;
        public string[] DocumentsID;

        public StageData_Old()
        {
        }
    }

    //=========================================================================

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

        [XmlArrayItem("Scripts")]
        public List<List<string>> FinishPriorThis;

        [XmlArrayItem("Set")]
        public List<List<string>> NextScripts;

        public KeyVal[] AccessoryData;

        public KeyVal[] Victims;
        public DeadBodyData GetVictimData(string name) => ParentCase.GetVictimData(Victims.FirstOrDefault(o => o.Key == name).Value);

        public KeyVal[] Evidence;
        public List<ObjectData> GetAllEvidenceData() => Evidence.Select(e => ParentCase.GetEvidenceData(e.Value)).ToList();

        public KeyVal[] Witnesses;
        public List<WitnessData> GetAllWitnesses() => Witnesses.Select(w => ParentCase.GetWitnessData(w.Value)).ToList();

        public KeyVal[] Officers;
        public FirstOfficerData GetOfficerData(string name)
        {
            return ParentCase.GetOfficerData(Officers.FirstOrDefault(o => o.Key == name).Value);
        }

        public KeyVal[] Coroners;
        public CoronerData GetCoronerData(string name) => GetData(name, (p, i) => p.GetCoronerData(i), Coroners);

        public KeyVal[] EMS;
        public EMSData GetEMSData(string name) => GetData(name, (p, i) => p.GetEMSData(i), EMS);

        public KeyVal[] Suspects;
        public SuspectData GetSuspectData(string name) => ParentCase.GetSuspectData(Dialogs.FirstOrDefault(s => s.Key == name).Value);

        public KeyVal[] Persons;
        public PersonData GetPersonData(string name) => ParentCase.GetPersonData(Persons.FirstOrDefault(s => s.Key == name).Value);

        public KeyVal[] Dialogs;
        public DialogData GetDialogData(string name) => ParentCase.GetDialogData(Dialogs.FirstOrDefault(d => d.Key == name).Value);

        public KeyVal[] Reports;
        public ReportData GetReportData(string name) => ParentCase.GetReportData(Reports.FirstOrDefault(r => r.Key == name).Value);

        public KeyVal[] Notes;
        public NoteData GetNoteData(string name) => ParentCase.GetNoteData(Notes.FirstOrDefault(n => n.Key == name).Value);

        public KeyVal[] Documents;
        public DocumentData GetDocuData(string name) => ParentCase.GetDocumentDataById(Documents.FirstOrDefault(d => d.Key == name).Value);

        public DocumentData GDD(string name) => GetData(name, (p, n) => p.GetDocumentDataById(n), Documents);

        private T GetData<T>(string name, Func<CaseData, string, T> getter, KeyVal[] src)
        {

            var key = src.FirstOrDefault(s => s.Key == name).Value;
            Game.LogTrivial($"{nameof(StageData.GetData)}: name:{name}, id:{key}");
            return getter(ParentCase, key);
        }

        public StageData()
        {
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
        public float Radius;
        public BlipSprite Sprite;
        public string Name;
        public string Color;

        //public Blip CreateBlip()
        //{
        //    Blip b = Radius == 0 ? new Blip(data.CallPosition) :
        //                                     new Blip(data.CallPosition, data.CallBlipRad);

        //    b.Sprite = data.CallBlipSprite;
        //    b.Name = data.CallBlipName;
        //    b.Color = ColorTranslator.FromHtml(data.CallBlipColor);

        //    return b;
        //}
    }








    //=========================================================================
    public class ResourceTypeDefinition
    {
        public Type Type;
        public string ResourceFilePath;

        public ResourceTypeDefinition(Type t, string p)
        {
            Type = t;
            ResourceFilePath = p;
        }
        public ResourceTypeDefinition() { } //required by serialization
    }

    public class ResourceTypes : List<ResourceTypeDefinition>
    {
        public string GetPath(Type t) => this.FirstOrDefault(s => s.Type == t).ResourceFilePath;
    }

    public class ResourceData
    {
        public string Name;
        public string Type;
        public string ResourceID;
    }

    public class StageData_REDUCED : IIdentifiable
    {
        public static ResourceTypeDefinition[] data = new[]
        {
            new ResourceTypeDefinition(typeof(DialogData), "")
        };

        public T GetResourceByName<T>(string name) where T : class, IIdentifiable
        {
            var id = Resources.FirstOrDefault(r => r.Name == name).ResourceID;
            var res = GetResourceByID<T>(id);
            return res;
        }
        //CaseData.ParentCase:
        public T GetResourceByID<T>(string id) where T : class, IIdentifiable
        {
            var path = data.FirstOrDefault(d => d.Type == typeof(T)).ResourceFilePath;
            var x = DataAccess.DataProvider.Instance.GetIdentifiableData<T>(path, id);

            return ProcessResult(x);
        }

        public T ProcessResult<T>(T x) where T: class,IIdentifiable
        {
            if (x is InterrogationData)
            {
                for (int i = 0; i < (x as InterrogationData).Lines.Length; i++)
                {
                    var e = (x as InterrogationData).Lines[i];

                    e.PlayerResponseTruth.RevalStrings();
                    e.Answer.RevalStrings();

                    e.InterrogeeReactionDoubt.RevalStrings();
                    e.InterrogeeReactionLie.RevalStrings();
                    e.InterrogeeReactionTruth.RevalStrings();

                    e.PlayerResponseDoubt.RevalStrings();
                    e.PlayerResponseLie.RevalStrings();
                    e.Question.RevalStrings();
                }
            }
            else if (x is DialogData)
            {
                (x as DialogData).Dialog.RevalStrings();
            }
            return x;
        }

        //STAGE LEVEL =/= CASE LEVEL!!!
        public IEnumerable<T> GetAllStageResourcesOfType<T>() where T : class, IIdentifiable
        {
            var e = Resources.Where(r => r.Type == typeof(T).Name);
            var f = e.Select(d => GetResourceByID<T>(d.ResourceID));
            return f;
        }

        public ResourceData[] Resources;

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

        [XmlArrayItem("Scripts")]
        public List<List<string>> FinishPriorThis;

        [XmlArrayItem("Set")]
        public List<List<string>> NextScripts;
        
        public StageData_REDUCED()
        {
        }
    }






}
