using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Windows;
using AudioScheduler.Model;
using NAudio.Wave;

namespace AudioScheduler
{
    public sealed class AudioController : INotifyPropertyChanged
    {
        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
        private readonly WaveOutEvent _waveOut = new WaveOutEvent();
        private string _playing = "None";

        public AudioController()
        {
            // Set event MediaPlayer and Synthesizer end
            _waveOut.PlaybackStopped += EndSound;
            _synthesizer.SpeakCompleted += EndSound;
        }

        // Implement INofityPropertChanged
        // ReSharper disable once MemberCanBePrivate.Global
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
            // Turn off stop event
            _waveOut.PlaybackStopped -= EndSound;
            _synthesizer.SpeakCompleted -= EndSound;

            // Stop file and TTS audio
            _waveOut.Stop();
            _synthesizer.SpeakAsyncCancelAll();

            // Turn on stop event
            _waveOut.PlaybackStopped += EndSound;
            _synthesizer.SpeakCompleted += EndSound;

            // Change what is playing
            Playing = "None";
        }

        // Play a specified Sound
        public void PlaySound(Sound sound)
        {
            // Check if same Sound already playing
            if (sound.Name == Playing)
            {
                App.Log($"Tried to play {sound.Name} while currently it was already playing");
                if (MessageBox.Show(
                        $"Are you sure you want to play {sound.Name} again? Since it is currently playing, it will restart from the beginning. Click No to cancel.",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
                App.Log($"Chose to restart {sound.Name} from beginning");
            }

            // Stop playing audio
            Stop();

            // Open and play the Sound
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
                App.ErrorMessage("Error playing sound.", e);
                Playing = "Error";
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

        // Called when a Sound or TTS finishes
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

        // Implement INofityPropertChanged
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}