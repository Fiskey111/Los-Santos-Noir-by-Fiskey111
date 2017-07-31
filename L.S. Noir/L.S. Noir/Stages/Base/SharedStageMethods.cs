using LSNoir.Data;
using Rage;
using System.Drawing;

namespace LSNoir.Stages.Base
{
    static class SharedStageMethods
    {
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

        public static void SaveRepNotEvdToProgress(StageData data)
        {
            data.ParentCase.AddNotesToProgress(data.NotesID);
            data.ParentCase.AddReportsToProgress(data.ReportsID);
            data.ParentCase.AddEvidenceToProgress(data.EvidenceID);
        }
    }
}
