using System;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;
using StardewValley;
using StardewValley.Menus;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Soundboard;

public partial class Sound
{
    private readonly Cue _cue;
    public readonly string Id;

    public readonly TimeSpan Duration;

    public string FormattedDuration;

    [Notify] public string durationRemaining;

    public readonly bool DoesLoop;

    public readonly long LoopCount;

    public bool IsModded;

    [Notify] private bool isPlaying;

    public bool ShouldDisplayToolTip
    {
        get
        {
            if (ModEntry.ModConfig.ForceTooltips) return true;
            
            var mainWidth = Game1.activeClickableMenu is null
                ? Game1.uiViewport.Width / 2 - 100
                : Game1.activeClickableMenu.width;
            var boxWidth = Math.Max(mainWidth - 64, 800) / 2f - 32;
            var maxTextFit = boxWidth - 24 - 36 - 16 - 8 - Soundboard.DURATION_WIDTH;
            return IdWidth > maxTextFit;
        }
    }

    public readonly int IdWidth;

    public string Tooltip => ShouldDisplayToolTip ? Id : string.Empty;

    public string Transform => IsPlaying ? "scale: 0.95" : "scale: 1";

    public bool useMilliseconds;

    public Sound(string id, Cue cue, float pitch = 1200f, bool milliseconds = false)
    {
        Id = id;
        _cue = cue;
        _cue.SetVariable("Pitch", pitch);
        var info = Soundboard.GetCueInfo(cue);
        Duration = info.Item1;
        DoesLoop = info.Item2;
        LoopCount = info.Item3;
        if (DoesLoop && LoopCount < 255)
        {
            Duration *= LoopCount + 1;
            DoesLoop = false; // It's only considered a "true loop" if it's specifically 255.
        }

        useMilliseconds = milliseconds;
        FormattedDuration = Duration == TimeSpan.Zero ? "(??:??)" :
            !useMilliseconds ? $"({Duration:mm\\:ss})" : $"({Duration:ss\\.ff})";
        durationRemaining = FormattedDuration;
        IsModded = Soundboard.IsCueModded(id);
        IdWidth = (int)Game1.smallFont.MeasureString(Id).X;
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
    }

    public void Stop()
    {
        if (_cue.IsPlaying) _cue.Stop(AudioStopOptions.Immediate);
        IsPlaying = false;
    }

    public bool IsCuePlaying()
    {
        return _cue.IsPlaying;
    }

    public void Update()
    {
        if (!IsPlaying)
        {
            DurationRemaining = FormattedDuration;
            return;
        }

        double time = _cue._time;
        if (time > Duration.TotalSeconds && !DoesLoop)
        {
            // Sometimes our time calculation is not 100% accurate, due to floating point inacurracy from the subtraction below
            // or because the Cue is being played at the hardcoded 1200f but in actuality it's meant to be played at a different pitch (and thus speed)
            // or because the Cue has multiple possible sounds in it that are different lengths...
            // or probably some other reason I haven't discovered yet. Point is, things can vary.
            // So just show 00:00 instead of a negative time or a weirdly counting /up/ time if our counter goes over our displayed duration.
            DurationRemaining = !useMilliseconds ? "(00:00)" : "(00.00)";
            return;
        }
        
        // _time will keep counting even after the Cue loops, it doesn't get reset
        // So we need to make sure it stays under the total Duration or else it'll go negative.
        while (time > Duration.TotalSeconds)
        {
            time -= Duration.TotalSeconds;
        }
        
        if (time < 0 || Duration == TimeSpan.Zero) return;

        TimeSpan remaining = Duration.Subtract(TimeSpan.FromSeconds(time));
        
        DurationRemaining = !useMilliseconds ? $"({remaining:mm\\:ss})" : $"({remaining:ss\\.ff})";
    }
}