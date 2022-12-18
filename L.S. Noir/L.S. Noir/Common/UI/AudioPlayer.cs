using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LSNoir.Common.UI
{
    internal class AudioPlayer
    {
        public static string SoundEffectsRoot = @"Plugins/LSPDFR/LSNoir/Audio/Sound Effects";

        public enum SoundEffectAudio { CameraShutter, CameraZoom, CaseArrive, Complete, Correct, Failed, Incorrect, NearbyEvidence }

        internal static void PlayAudio(SoundEffectAudio audio)
        {
            string audioFilePath = string.Empty;
            if (File.Exists($"{SoundEffectsRoot}/{audio}.wav"))
            {
                audioFilePath = $"{SoundEffectsRoot}/{audio}.wav";
            }
            else if (File.Exists($"{SoundEffectsRoot}/{audio}.mp3"))
            {
                audioFilePath = $"{SoundEffectsRoot}/{audio}.mp3";
            }

            if (string.IsNullOrWhiteSpace(audioFilePath))
            {
                Logger.LogDebug(nameof(AudioPlayer), nameof(PlayAudio), $"Audio file not found: {SoundEffectsRoot}/{audio}");
                return;
            }

            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(Path.GetFullPath(audioFilePath)));
            while (!player.HasAudio || player.IsBuffering)
                GameFiber.Yield();
            player.Play();
        }
    }
}
