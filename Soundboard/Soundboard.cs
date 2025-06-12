using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;
using StardewValley;
using StardewValley.GameData;

namespace Soundboard;

public partial class Soundboard
{
    public partial class Sound
    {
        private readonly ICue _cue;
        public string Id { get; }
        public uint Category { get; }
        public string CategoryString => GetCategoryName();
        [Notify] public bool isPlaying;
        
        public Sound(string id, uint category, ICue cue)
        {
            Id = id;
            Category = category;
            _cue = cue;
        }
        
        public string GetCategoryName()
        {
            return Category switch
            {
                1 => "Default",
                2 => "Music",
                3 => "Sound Effects",
                4 => "Ambient",
                5 => "Footsteps",
                _ => "Unknown"
            };
        }

        public void ToggleState()
        {
            if (_cue.IsPlaying) Stop();
            else Play();
        }

        public void Play()
        {
            if (_cue.IsPlaying || IsPlaying) _cue.Stop(AudioStopOptions.Immediate);
            _cue.Play();
            IsPlaying = true;
            if (!PlayingSounds.Contains(this))
            {
                PlayingSounds.Add(this);
            }
        }

        public void Stop()
        {
            if (_cue.IsPlaying) _cue.Stop(AudioStopOptions.Immediate);
            IsPlaying = false;
            if (PlayingSounds.Contains(this))
            {
                PlayingSounds.Remove(this);
            }
        }

        public bool IsCuePlaying()
        {
            return _cue.IsPlaying;
        }
    }

    public Dictionary<uint, List<Sound>> Sounds { get; } = new Dictionary<uint, List<Sound>>()
    {
        { 1, new List<Sound>() }, // Default
        { 2, new List<Sound>() }, // Music
        { 3, new List<Sound>() }, // Sound Effects
        { 4, new List<Sound>() }, // Ambient
        { 5, new List<Sound>() } // Footsteps
    };
    
    public List<Sound> Default => Sounds[1];
    public List<Sound> Music => Sounds[2];
    public List<Sound> SoundEffects => Sounds[3];
    public List<Sound> Ambient => Sounds[4];
    public List<Sound> Footsteps => Sounds[5];

    public bool IsOpen = false;
    
    public static List<Sound> PlayingSounds { get; } = new List<Sound>();
    
    public Soundboard()
    {
        GetCues();
    }
    
    public void GetCues()
    {
        var soundBank = (Game1.soundBank as SoundBankWrapper)?.soundBank;
        if (soundBank == null) return;

        var cues = soundBank._cues;
            
        foreach (var cue in cues.Values.OrderBy(cue => cue.name))
        {
            Log.Warn($"Cue: {cue.name} - Category: {string.Join(",", cue.sounds.Select(sound => sound.categoryID).Distinct())}");
            uint category = cue.sounds.Select(sound => sound.categoryID).FirstOrDefault();
            if (!Sounds.ContainsKey(category))
            {
                Sounds[category] = new List<Sound>();
            }

            Sounds[category].Add(new Sound(cue.name, category, Game1.soundBank.GetCue(cue.name)));
        }
            
        Log.Info($"Loaded {cues.Count} sound cues into the soundboard.");
    }

    public void OpenSoundboard()
    {
        Game1.changeMusicTrack("none");
        var context = new
        {
            Soundboard = this,
            HeaderText = "Soundboard",
        };
        Game1.activeClickableMenu = ModEntry.viewEngine?.CreateMenuFromAsset($"{ModEntry.Prefix}/Views/Soundboard", context);
    }

    public void Update()
    {
        foreach (var sound in PlayingSounds.ToList().Where(sound => !sound.IsCuePlaying()))
        {
            sound.Stop();
        }
    }
}