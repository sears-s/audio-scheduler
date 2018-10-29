using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;

namespace AudioScheduler.Model
{
    public class Sound
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; private set; }

        [Required] public string FilePath { get; private set; }

        // Add a Sound from a file
        public static void AddFile(string name, string path)
        {
            // Check if not .mp3 or .wav
            if (!(Path.GetExtension(path) == ".mp3" || Path.GetExtension(path) == ".wav"))
            {
                App.ErrorMessage("Only .mp3 and .wav audio files are permitted.");
                return;
            }

            using (var db = new Context())
            {
                // Check if name is duplicate
                if (db.Sounds.Any(o => o.Name.Equals(name)))
                {
                    App.ErrorMessage($"Sound named {name} already exists.");
                    return;
                }

                // Create Sound
                var newSound = new Sound
                {
                    Name = name,
                    FilePath = Path.Combine(App.SoundDirectory, name + Path.GetExtension(path))
                };

                // Copy the file
                try
                {
                    File.Copy(path, newSound.FilePath);
                }
                catch (Exception e)
                {
                    App.ErrorMessage($"Error copying {path} to {Path.GetFullPath(newSound.FilePath)}.", e);
                    return;
                }

                // Add Sound to database
                db.Sounds.Add(newSound);
                db.SaveChanges();
            }
        }

        // Add TTS Sound to database
        public static void AddTts(string name, string speech, string voice)
        {
            using (var db = new Context())
            {
                // Check if name is duplicate
                if (db.Sounds.Any(o => o.Name.Equals(name)))
                {
                    App.ErrorMessage($"Sound named {name} already exists.");
                    return;
                }

                // Set path
                var path = Path.Combine(App.SoundDirectory, name + ".wav");

                // Create TTS WAV file
                var synthesizer = new SpeechSynthesizer
                {
                    Volume = 100,
                    Rate = -2
                };
                try
                {
                    synthesizer.SelectVoice(voice);
                    synthesizer.SetOutputToWaveFile(path);
                    synthesizer.Speak(speech);
                }
                catch (Exception e)
                {
                    App.ErrorMessage("Error creating TTS file from input.", e);

                    // Delete the file if it was created
                    if (File.Exists(path)) File.Delete(path);

                    return;
                }
                
                // Dispose the SpeechSynthesizer
                synthesizer.Dispose();

                // Create Sound
                var newSound = new Sound
                {
                    Name = name,
                    FilePath = path
                };

                // Add Sound to database
                db.Sounds.Add(newSound);
                db.SaveChanges();
            }
        }

        // Remove Sound given its id
        public static void Remove(int id)
        {
            using (var db = new Context())
            {
                // Find Sound with id
                var sound = db.Sounds.Find(id);
                if (sound == null)
                {
                    App.ErrorMessage($"Could not find sound with ID #{id} in database.");
                    return;
                }

                // Delete Sound from database
                db.Sounds.Remove(sound);
                db.SaveChanges();

                // Delete the file
                try
                {
                    File.Delete(sound.FilePath);
                }
                catch (Exception e)
                {
                    App.ErrorMessage($"Error deleting file {Path.GetFullPath(sound.FilePath)}.", e);
                }
            }
        }

        // Rename a Sound
        public static void Rename(int id, string name)
        {
            using (var db = new Context())
            {
                // Check if name is duplicate
                if (db.Sounds.Any(o => o.Name.Equals(name)))
                {
                    App.ErrorMessage($"Sound named {name} already exists.");
                    return;
                }

                // Find Sound with id
                var sound = db.Sounds.Find(id);
                if (sound == null)
                {
                    App.ErrorMessage($"Could not find sound with ID #{id} in database.");
                    return;
                }

                // Rename in database
                sound.Name = name;
                var oldPath = sound.FilePath;
                sound.FilePath = Path.Combine(App.SoundDirectory, name + ".mp3");
                db.SaveChanges();

                // Rename the file
                try
                {
                    File.Move(oldPath, sound.FilePath);
                }
                catch (Exception e)
                {
                    sound.FilePath = oldPath;
                    db.SaveChanges();
                    App.ErrorMessage(
                        $"Error renaming {Path.GetFullPath(oldPath)} to {Path.GetFullPath(sound.FilePath)}.", e);
                }
            }
        }

        // Returns all Sounds, sorted alphabetically
        public static IEnumerable<Sound> Fetch(Context db = null)
        {
            if (db != null) return db.Sounds.OrderBy(o => o.Name).ToList();
            {
                using (db = new Context())
                {
                    return db.Sounds.OrderBy(o => o.Name).ToList();
                }
            }
        }
    }
}