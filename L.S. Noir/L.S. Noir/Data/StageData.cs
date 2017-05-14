using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LSNoir.Data
{
    //TODO:
    // - make fields readonly to disable changes?
    public class StageData : IIdentifiable
    {
        public string ID { get; set; }
        public string StageType;
        [NonSerialized]
        public CaseData ParentCase; //TODO: replace with caseID? + GetParentCase()
        public Vector3 CallPosition;
        public float CallBlipRad;
        public bool PlaySoundClosingIn;
        public string SceneID;

        public string MsgSuccess;
        public string MsgFailure; //used in failure screen

        public string[] NextScripts = new string[] { };

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

        public BlipSprite CallBlipSprite;
        public string CallBlipName;

        public string VictimID;
        public string[] EvidenceID;
        public string[] WitnessID;
        public string OfficerID;
        public string CoronerID;
        public string EmsID;

        public string[] DialogsID;
        public string[] ReportsID;
        public string[] NotesID;

        public StageData()
        {
        }
    }
}
