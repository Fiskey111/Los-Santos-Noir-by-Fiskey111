using LSNoir.Data;
using LSNoir.Scenes;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System.Drawing;
using System.Linq;

namespace LSNoir.Stages.Base
{
    static class SharedStageMethods
    {
        public static IScene GetScene(StageData data)
        {
            string sceneId = data.SceneID;
            if (string.IsNullOrEmpty(sceneId)) return null;

            SceneData sd = data.ParentCase.GetResourceByID<SceneData>(sceneId);
            return sd.GetScene();
        }

        //public static void DisplayNotification(StageData data)
        //{
        //    var 
        //    Game.DisplayNotification(data.NotificationTexDic, data.NotificationTexName,
        //        data.NotificationTitle, data.NotificationSubtitle, data.NotificationText);
        //}

        public static Blip CreateBlip(StageData data)
        {
            Blip b = data.CallBlip.Radius == 0 ? new Blip(data.CallPosition) : 
                                             new Blip(data.CallPosition, data.CallBlip.Radius);
            
            b.Sprite = data.CallBlip.Sprite;
            b.Name = data.CallBlip.Name;
            b.Color = ColorTranslator.FromHtml(data.CallBlip.Color);

            return b;
        }
        //TODO: move to ProgressHelper!
        public static void SaveRepNotEvdToProgress(StageData data)
        {
            data.ParentCase.Progress.AddNotesToProgress(data.GetAllIDsOfType<NoteData>().ToArray());
            data.ParentCase.Progress.AddReportsToProgress(data.GetAllIDsOfType<ReportData>().ToArray());
            data.ParentCase.Progress.AddEvidenceToProgress(data.GetAllIDsOfType<ObjectData>().ToArray());
        }
    }
}
