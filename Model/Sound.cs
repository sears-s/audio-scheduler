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

        // Add sound to database
        public static void AddFile(string name, string path)
        {
            // Check if not .mp3 or .wav
            if (!(Path.GetExtension(path) == ".mp3" || Path.GetExtension(path) == ".wav"))
            {
                App.InfoMessage("Incorrect File Extension", "Only .mp3 and .wav audio files are permitted.");
                return;
            }

            try
            {
                using (var db = new Context())
                {
                    // Check if name is duplicate
                    if (db.Sounds.Any(o => o.Name.Equals(name)))
                    {
                        App.InfoMessage("Duplicate Sound", $"Sound {name} already exists in sounds database.");
                        return;
                    }

                    // Create sound object
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

                    // Add newSound to database
                    db.Sounds.Add(newSound);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error adding sound to database.", e);
            }
        }

        // Add TTS sound to database
        public static void AddTts(string name, string speech, string voice)
        {
            try
            {
                using (var db = new Context())
                {
                    // Check if name is duplicate
                    if (db.Sounds.Any(o => o.Name.Equals(name)))
                    {
                        App.InfoMessage("Duplicate Sound", $"Sound {name} already exists in sounds database.");
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

                    // Create sound object
                    var newSound = new Sound
                    {
                        Name = name,
                        FilePath = path
                    };

                    // Add newSound to database
                    db.Sounds.Add(newSound);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error adding sound to database.", e);
            }
        }

        // Remove sound from database
        public static void Remove(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    // Find sound with id
                    var sound = db.Sounds.Find(id);
                    if (sound == null)
                    {
                        App.ErrorMessage($"Could not find sound with ID #{id} in database.");
                        return;
                    }

                    // Delete sound from database
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
            catch (Exception e)
            {
                App.ErrorMessage("Error removing sound from database.", e);
            }
        }

        // Rename sound in database
        public static void Rename(int id, string name)
        {
            try
            {
                using (var db = new Context())
                {
                    // Find sound with id
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
                            $"Error renaming {Path.GetFullPath(oldPath)} to {Path.GetFullPath(sound.FilePath)}.",
                            e);
                    }
                }
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error renaming sound in database.", e);
            }
        }

        // Return all Sounds in database, sorted alphabetically, ascending
        public static List<Sound> Fetch(Context db = null)
        {
            // Create list
            var result = new List<Sound>();

            // Fetch data
            try
            {
                if (db == null)
                    using (db = new Context())
                    {
                        result.AddRange(db.Sounds);
                    }
                else
                    result.AddRange(db.Sounds);
            }
            catch (Exception e)
            {
                App.ErrorMessage("Error fetching sounds from database.", e);
                return null;
            }

            // Return sorted list
            return result.OrderBy(o => o.Name).ToList();
        }
    }
}