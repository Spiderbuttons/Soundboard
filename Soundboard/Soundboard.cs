using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Menus;

// ReSharper disable MemberCanBePrivate.Global

namespace Soundboard;

public partial class Soundboard
{
    public const int DURATION_WIDTH = 76;
    
    public static Dictionary<string, AudioCueData>? _cueChanges;

    private static Dictionary<string, AudioCueData> CueChanges { get; } =
        _cueChanges ??= DataLoader.AudioChanges(Game1.content);

    private readonly SoundList Default = new ("Default");
    private readonly SoundList Music = new ("Music");
    private readonly SoundList SoundEffects = new ("Sound Effects");
    private readonly SoundList Ambient = new ("Ambient");
    private readonly SoundList Footsteps = new ("Footsteps");

    [Notify] public string selectedCategory = "Music";

    public SoundList SelectedList => SelectedCategory switch
    {
        "Music" => Music,
        "Sound Effects" => SoundEffects,
        "Ambient" => Ambient,
        "Footsteps" => Footsteps,
        _ => Default
    };
    
    public string CategoryName => ModEntry.ModHelper.Translation.Get(SelectedList.Category);

    public bool IsOpen;

    public static IClickableMenu? SoundboardMenu;

    public Soundboard()
    {
        GetCues();
    }

    public void GoToNextCategory(int direction)
    {
        SelectedCategory = SelectedCategory switch
        {
            "Music" => direction > 0 ? "Sound Effects" : "Footsteps",
            "Sound Effects" => direction > 0 ? "Ambient" : "Music",
            "Ambient" => direction > 0 ? "Footsteps" : "Sound Effects",
            "Footsteps" => direction > 0 ? "Music" : "Ambient",
            _ => "Music"
        };

        Game1.playSound("shwip");
    }

    private void GetCues()
    {
        var cues = (Game1.soundBank as SoundBankWrapper)!.soundBank._cues.Keys.ToList();
        cues.Sort();

        foreach (var cueName in cues)
        {
            // ModEntry.VanillaSoundBank.Exists(cueName) && !CueChanges.ContainsKey(cueName) ? ModEntry.VanillaSoundBank.GetCue(cueName) : 
            Cue? q = (Game1.soundBank.GetCue(cueName) as CueWrapper)?.cue;
            if (q == null) continue;

            uint category = q._cueDefinition.sounds.Select(sound => sound.categoryID).FirstOrDefault();
            var sound = new Sound(cueName, q, pitch: 1200f, milliseconds: category is 3 or 5);

            switch (category)
            {
                case 1:
                    Default.AddSound(sound);
                    break;
                case 2:
                    Music.AddSound(sound);
                    break;
                case 3:
                    SoundEffects.AddSound(sound);
                    break;
                case 4:
                    Ambient.AddSound(sound);
                    break;
                case 5:
                    Footsteps.AddSound(sound);
                    break;
            }
        }

        Log.Info($"Loaded {cues.Count} sound cues into the soundboard.");
    }

    public static (TimeSpan, bool, long) GetCueInfo(Cue cue)
    {
        return (GetCueDuration(cue), DoesCueLoop(cue, out var loopCount), loopCount);
    }

    private static TimeSpan GetCueDuration(Cue cue)
    {
        var fx = cue.GetPlayWaveEvents()?.FirstOrDefault()?._variants?.FirstOrDefault()?.GetSoundEffect() ?? cue._xactSounds?.FirstOrDefault()?.GetSimpleSoundInstance()?._effect;
        return fx switch
        {
            OggStreamSoundEffect ogg => SoundEffect.GetSampleDuration((int)ogg.TotalSamplesPerChannel * OggStreamSoundEffect.BytesPerSample * 2, ogg.SampleRate, AudioChannels.Stereo),
            not null => SoundEffect.GetSampleDuration(fx.Spring?.Length ?? 0, fx.Spring?.SampleRate ?? 44125, AudioChannels.Stereo),
            _ => TimeSpan.Zero
        };
    }

    private static bool DoesCueLoop(Cue cue, out long loopCount)
    {
        loopCount = cue.GetPlayWaveEvents()?.Max(e => e._loopCount) ?? 0;
        return loopCount > 0;
    }

    public static bool IsCueModded(string cueName)
    {
        return CueChanges.ContainsKey(cueName) || !ModEntry.VanillaSoundBank.Exists(cueName);
    }

    public bool HandleButton(SButton button)
    {
        return ChangeCategory(button) || ChangePage(button);
    }

    private bool ChangeCategory(SButton button)
    {
        int direction = button switch
        {
            SButton.LeftTrigger => -1,
            SButton.RightTrigger => 1,
            _ => 0
        };

        if (direction == 0) return false;
        GoToNextCategory(direction);
        return true;
    }

    private bool ChangePage(SButton button)
    {
        int direction = button switch
        {
            SButton.LeftShoulder => -1,
            SButton.RightShoulder => 1,
            _ => 0
        };

        if (direction == 0) return false;
        SelectedList.SwitchPage(direction);
        return true;
    }

    public void PrepareSoundboard()
    {
        SoundboardMenu = ModEntry.viewEngine?.CreateMenuFromAsset($"{ModEntry.Prefix}/Views/Soundboard", new
        {
            Soundboard = this,
        });
        SoundboardMenu!.exitFunction = CloseSoundboard;
    }

    public void CloseSoundboard()
    {
        SoundboardMenu?.exitThisMenu();
        Default.StopAllSounds();
        Music.StopAllSounds();
        SoundEffects.StopAllSounds();
        Ambient.StopAllSounds();
        Footsteps.StopAllSounds();
        Game1.changeMusicTrack("none");
        IsOpen = false;
    }

    public void OpenSoundboard()
    {
        if (SoundboardMenu == null)
            PrepareSoundboard();
        
        Game1.changeMusicTrack("none");
        Game1.activeClickableMenu = SoundboardMenu;
        IsOpen = true;
        Game1.playSound("bigSelect");
    }
}