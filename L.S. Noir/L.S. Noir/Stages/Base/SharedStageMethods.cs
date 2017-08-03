using LSNoir.Data;
using LSNoir.Scenes;
using Rage;
using System.Drawing;

namespace LSNoir.Stages.Base
{
    static class SharedStageMethods
    {
        public static IScene GetScene(StageData data)
        {
            string sceneId = data.SceneID;
            if (string.IsNullOrEmpty(sceneId)) return null;

            SceneData sd = data.ParentCase.GetSceneData(sceneId);
            return sd.GetScene();
        }

        public static void DisplayNotification(StageData data)
        {
            Game.DisplayNotification(data.NotificationTexDic, data.NotificationTexName,
                data.NotificationTitle, data.NotificationSubtitle, data.NotificationText);
        }

        public static Blip CreateBlip(StageData data)
        {
            Blip b = data.CallBlipRad == 0 ? new Blip(data.CallPosition) : 
                                             new Blip(data.CallPosition, data.CallBlipRad);
            
            b.Sprite = data.CallBlipSprite;
            b.Name = data.CallBlipName;
            b.Color = ColorTranslator.FromHtml(data.CallBlipColor);

            return b;
        }
        //TODO: move to ProgressHelper!
        public static void SaveRepNotEvdToProgress(StageData data)
        {
            data.ParentCase.Progress.AddNotesToProgress(data.NotesID);
            data.ParentCase.Progress.AddReportsToProgress(data.ReportsID);
            data.ParentCase.Progress.AddEvidenceToProgress(data.EvidenceID);
        }
    }
}
