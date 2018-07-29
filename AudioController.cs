using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using AudioScheduler.Model;
using NAudio.Wave;

namespace AudioScheduler
{
    public static class AudioController
    {
        // Properties
        private static readonly WaveOutEvent WaveOut = new WaveOutEvent();
        private static readonly SpeechSynthesizer Synthesizer = new SpeechSynthesizer();

        // Initialization of static class
        static AudioController()
        {
            // Go to event when MediaPlayer and Synthesizer end
            WaveOut.PlaybackStopped += EndSound;
            Synthesizer.SpeakCompleted += EndSound;
        }

        public static string Playing { get; private set; } = "None";

        // Stop what is playing
        public static void Stop()
        {
            // Stop file and TTS audio
            WaveOut.Stop();
            Synthesizer.SpeakAsyncCancelAll();

            // Change what is playing
            Playing = "None";
        }

        // Play a specified sound
        public static void PlaySound(Sound sound)
        {
            // Stop playing audio
            Stop();

            // Open and play the sound
            try
            {
                using (var reader = new MediaFoundationReader(sound.FilePath))
                {
                    Playing = "Loading";
                    WaveOut.Init(reader);
                    WaveOut.Play();
                    Playing = sound.Name;
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage($"Error playing {sound.Name} sound.", e);
                Playing = "None";
            }
        }

        // Play a TTS
        public static void PlayTts(string text, string voice)
        {
            // Stop playing audio
            Stop();

            // Change the voice
            Synthesizer.SelectVoice(voice);

            // Speak the text
            Synthesizer.SpeakAsync(text);
            Playing = "Custom TTS";
        }

        // Called when a sound or TTS finishes
        private static void EndSound(object sender, EventArgs e)
        {
            // Reset what is playing
            Playing = "None";
        }

        // Returns of a list of all the synthesizer voices names
        public static List<string> GetVoices()
        {
            return Synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo).Select(v => v.Name).ToList();
        }
    }
}