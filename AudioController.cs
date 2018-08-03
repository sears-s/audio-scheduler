using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using AudioScheduler.Model;
using NAudio.Wave;

namespace AudioScheduler
{
    public sealed class AudioController : INotifyPropertyChanged
    {
        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
        private readonly WaveOutEvent _waveOut = new WaveOutEvent();

        private string _playing = "None";

        // Initialization of static class
        public AudioController()
        {
            // Go to event when MediaPlayer and Synthesizer end
            _waveOut.PlaybackStopped += EndSound;
            _synthesizer.SpeakCompleted += EndSound;
        }

        public string Playing
        {
            get => _playing;
            private set
            {
                _playing = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Stop what is playing
        public void Stop()
        {
            // Stop file and TTS audio
            _waveOut.Stop();
            _synthesizer.SpeakAsyncCancelAll();

            // Change what is playing
            Playing = "None";
        }

        // Play a specified sound
        public void PlaySound(Sound sound)
        {
            // Stop playing audio
            Stop();

            // Open and play the sound
            try
            {
                using (var reader = new MediaFoundationReader(sound.FilePath))
                {
                    Playing = "Loading";
                    _waveOut.Init(reader);
                    _waveOut.Play();
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
        public void PlayTts(string text, string voice)
        {
            // Stop playing audio
            Stop();

            // Change the voice
            _synthesizer.SelectVoice(voice);

            // Speak the text
            _synthesizer.SpeakAsync(text);
            Playing = "Custom TTS";
        }

        // Called when a sound or TTS finishes
        private void EndSound(object sender, EventArgs e)
        {
            // Reset what is playing
            Playing = "None";
        }

        // Returns of a list of all the synthesizer voices names
        public IEnumerable<string> GetVoices()
        {
            return _synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo).Select(v => v.Name).ToList();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}