using System;
using System.IO;
using System.Windows.Media;
using LSNoir.Extensions;
using Rage;
using RAGENativeUI.Elements;

namespace LSNoir.Callouts.Universal
{
    class BigMessage
    {
        private static BigMessageThread bigMessage;

        public static void Main()
        {
            GameFiber.StartNew(delegate
            {
                MediaPlayer m = new MediaPlayer();
                m.Open(new Uri(Path.GetFullPath(@"Plugins/LSPDFR/LSNoir/Audio/Complete.wav")));
                m.HasAudio.ToString().AddLog();
                while (!m.HasAudio || m.IsBuffering)
                    GameFiber.Yield();
                "Playing audio".AddLog();
                m.Position = TimeSpan.Zero;
                m.NaturalDuration.ToString().AddLog();
                m.Play();
            });

            bigMessage = new BigMessageThread(true);
            var c = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(LSNoir.Main.CDataPath);
            bigMessage.MessageInstance.ShowMissionPassedMessage("Case #" + c.Number + " Completed!");
        }
    }
}
