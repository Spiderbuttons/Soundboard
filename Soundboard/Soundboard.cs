using System;
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
        private readonly Cue _cue;
        public string Id;
        public uint Category;
        public string CategoryString => GetCategoryName();

        public TimeSpan Duration;

        public string FormattedDuration;// => $"({Duration:mm\\:ss})";

        public bool DoesLoop;

        public bool IsModded;
        
        [Notify] public bool isPlaying;
        
        public Sound(string id, uint category, Cue cue)
        {
            Id = id;
            Category = category;
            _cue = cue;
            Duration = GetCueDuration(cue);
            FormattedDuration = Duration == TimeSpan.Zero ? "(??:??)" : $"({Duration:mm\\:ss})";
            DoesLoop = DoesCueLoop(cue);
            IsModded = IsCueModded(id);
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
            if (_cue.IsPlaying == true) Stop();
            else Play();
        }

        public void Play()
        {
            if (_cue.IsPlaying || IsPlaying) _cue.Stop(AudioStopOptions.Immediate);
            _cue?.Play();
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

    public static Dictionary<string, AudioCueData>? _cueChanges = null;

    public static Dictionary<string, AudioCueData> CueChanges { get; } =
        _cueChanges ??= DataLoader.AudioChanges(Game1.content);

    public static Dictionary<uint, List<Sound>> Sounds { get; } = new Dictionary<uint, List<Sound>>()
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

    [Notify] public string selectedCategory = "Music";

    [Notify] public List<Sound> selectedList = Sounds[2];
    
    public bool IsOpen = false;
    
    public static List<Sound> PlayingSounds { get; } = new List<Sound>();
    
    public Soundboard()
    {
        GetCues();
    }
    
    public void GoToNextCategory() 
    {
        if (selectedCategory == "Default")
        {
            SelectedCategory = "Music";
            SelectedList = Music;
        } 
        else if (selectedCategory == "Music")
        {
            SelectedCategory = "Sound Effects";
            SelectedList = SoundEffects;
        }
        else if (selectedCategory == "Sound Effects")
        {
            SelectedCategory = "Ambient";
            SelectedList = Ambient;
        }
        else if (selectedCategory == "Ambient")
        {
            SelectedCategory = "Footsteps";
            SelectedList = Footsteps;
        }
        else if (selectedCategory == "Footsteps")
        {
            SelectedCategory = "Music";
            SelectedList = Music;
        }
        
        Game1.playSound("shwip");
    }
    
    public void GetCues()
    {
        var soundBank = (Game1.soundBank as SoundBankWrapper)?.soundBank;
        if (soundBank == null) return;

        var cues = soundBank._cues;

        foreach (var kvp in cues.OrderBy(kvp => kvp.Key))
        {
            var cue = kvp.Value;
            var id = kvp.Key;
            uint category = cue.sounds.Select(sound => sound.categoryID).FirstOrDefault();
            if (!Sounds.ContainsKey(category))
            {
                Sounds[category] = new List<Sound>();
            }
            
            CueWrapper? wrapper = Game1.soundBank.GetCue(cue.name) as CueWrapper;
            if (wrapper == null) continue;
            Sounds[category].Add(new Sound(id, category, wrapper.cue));
        }
            
        Log.Info($"Loaded {cues.Count} sound cues into the soundboard.");
    }
    
    public static TimeSpan GetCueDuration(Cue cue)
    {
        try
        {
            var instance = cue._soundEffect ?? cue._xactSounds?.FirstOrDefault()?.GetSimpleSoundInstance() ??
                cue.GetPlayWaveEvents()?.FirstOrDefault()?._variants?.FirstOrDefault()
                    ?.GetSoundEffectInstance();
            
            if (instance == null) return TimeSpan.Zero;
            
            return SoundEffect.GetSampleDuration(instance._effect.Spring.Length, 44125, AudioChannels.Stereo);
        }
        catch (Exception e)
        {
            return TimeSpan.Zero;
        }
    }

    public static bool DoesCueLoop(Cue cue)
    {
        if (cue._xactSounds[0].complexSound)
        {
            var instance = cue.GetPlayWaveEvents()[0]._variants[0].GetSoundEffectInstance();
            return instance.PlatformGetIsLooped();
        }
        else
        {
            var instance = cue._xactSounds[0].GetSimpleSoundInstance();
            return instance.PlatformGetIsLooped();
        }
    }
    
    public static bool IsCueModded(string cueName)
    {
        return CueChanges.ContainsKey(cueName);
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
        Game1.activeClickableMenu!.exitFunction = () =>
        {
            Game1.changeMusicTrack("none");
            IsOpen = false;
            PlayingSounds.Clear();
        };
        IsOpen = true;
    }

    public void OpenTestBoard()
    {
        Game1.changeMusicTrack("none");
        var context = new
        {
            Soundboard = this,
            HeaderText = "Test Board",
        };
        Game1.activeClickableMenu = ModEntry.viewEngine?.CreateMenuFromAsset($"{ModEntry.Prefix}/Views/Testboard", context);
        Game1.activeClickableMenu!.exitFunction = () =>
        {
            IsOpen = false;
            PlayingSounds.Clear();
        };
        IsOpen = true;
    }

    public void Update()
    {
        foreach (var sound in PlayingSounds.ToList().Where(sound => !sound.IsCuePlaying()))
        {
            sound.Stop();
        }
    }
}