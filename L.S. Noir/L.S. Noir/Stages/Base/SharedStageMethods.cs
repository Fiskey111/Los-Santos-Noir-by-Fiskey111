using LSNoir.Data;
using Rage;

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
            Blip b;
            if (data.CallBlipRad == 0)
            {
                b = new Blip(data.CallPosition);
            }
            else
            {
                b = new Blip(data.CallPosition, data.CallBlipRad);
            }
            b.Sprite = data.CallBlipSprite;
            b.Name = data.CallBlipName;
            //b.Color
            return b;
        }
    }
}
