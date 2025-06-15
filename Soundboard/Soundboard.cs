using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace Soundboard;

public partial class Soundboard
{
    public static Dictionary<string, AudioCueData>? _cueChanges = null;

    public static Dictionary<string, AudioCueData> CueChanges { get; } =
        _cueChanges ??= DataLoader.AudioChanges(Game1.content);

    public SoundList Default = new SoundList("Default");
    public SoundList Music = new SoundList("Music");
    public SoundList SoundEffects = new SoundList("Sound Effects");
    public SoundList Ambient = new SoundList("Ambient");
    public SoundList Footsteps = new SoundList("Footsteps");

    [Notify] public string selectedCategory = "Music";

    public SoundList SelectedList => SelectedCategory switch
    {
        "Music" => Music,
        "Sound Effects" => SoundEffects,
        "Ambient" => Ambient,
        "Footsteps" => Footsteps,
        _ => Default
    };

    public bool IsOpen = false;

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

    public void GetCues()
    {
        var cues = (Game1.soundBank as SoundBankWrapper)!.soundBank._cues.Keys.ToList();
        cues.Sort();

        foreach (var cueName in cues)
        {
            Cue? q = ModEntry.VanillaSoundBank.Exists(cueName) ? ModEntry.VanillaSoundBank.GetCue(cueName) : (Game1.soundBank.GetCue(cueName) as CueWrapper)?.cue;
            if (q == null) continue;

            uint category = q._cueDefinition.sounds.Select(sound => sound.categoryID).FirstOrDefault();
            var sound = new Sound(cueName, q, milliseconds: category is 3 or 5);

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

    public static TimeSpan GetCueDuration(Cue cue)
    {
        var fx = cue.GetPlayWaveEvents()?.FirstOrDefault()?._variants?.FirstOrDefault()?.GetSoundEffect() ?? cue._xactSounds?.FirstOrDefault()?.GetSimpleSoundInstance()?._effect;
        return fx switch
        {
            OggStreamSoundEffect ogg => SoundEffect.GetSampleDuration((int)ogg.TotalSamplesPerChannel * OggStreamSoundEffect.BytesPerSample * 2, ogg.SampleRate, AudioChannels.Stereo),
            not null => SoundEffect.GetSampleDuration(fx?.Spring?.Length ?? 0, fx?.Spring?.SampleRate ?? 44125, AudioChannels.Stereo),
            _ => TimeSpan.Zero
        };
    }

    public static bool DoesCueLoop(Cue cue, out long loopCount)
    {
        loopCount = cue.GetPlayWaveEvents()?.Max(e => e._loopCount) ?? 0;
        return loopCount > 0;

        // return events is not null && events.Any(e => e is not null && e._loopCount > 0);
        if (cue._xactSounds[0].complexSound)
        {
            var instance = cue.GetPlayWaveEvents()[0]._variants[0].GetSoundEffectInstance();
            return instance.LoopCount > 0;
        }
        else
        {
            var instance = cue._xactSounds[0].GetSimpleSoundInstance();
            return instance.LoopCount > 0;
        }
    }

    public static bool IsCueModded(string cueName)
    {
        return CueChanges.ContainsKey(cueName) || !ModEntry.VanillaSoundBank.Exists(cueName);
    }

    public bool HandleButton(SButton button)
    {
        return ChangeCategory(button) || ChangePage(button);
    }

    public bool ChangeCategory(SButton button)
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

    public bool ChangePage(SButton button)
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

    public void OpenSoundboard()
    {
        Game1.changeMusicTrack("none");
        var context = new
        {
            Soundboard = this,
            HeaderText = "Soundboard",
        };
        Game1.activeClickableMenu =
            ModEntry.viewEngine?.CreateMenuFromAsset($"{ModEntry.Prefix}/Views/Soundboard", context);
        Game1.activeClickableMenu!.exitFunction = () =>
        {
            Game1.changeMusicTrack("none");
            IsOpen = false;
            Default.StopAllSounds();
            Music.StopAllSounds();
            SoundEffects.StopAllSounds();
            Ambient.StopAllSounds();
            Footsteps.StopAllSounds();
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
        Game1.activeClickableMenu =
            ModEntry.viewEngine?.CreateMenuFromAsset($"{ModEntry.Prefix}/Views/Testboard", context);
        Game1.activeClickableMenu!.exitFunction = () =>
        {
            Game1.changeMusicTrack("none");
            IsOpen = false;
            Default.StopAllSounds();
            Music.StopAllSounds();
            SoundEffects.StopAllSounds();
            Ambient.StopAllSounds();
            Footsteps.StopAllSounds();
        };
        IsOpen = true;
    }

    public void Update()
    {
        // Log.Warn("Updating Soundboard");
    }
}