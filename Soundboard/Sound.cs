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

    public readonly bool DoesLoop;

    public readonly long LoopCount;

    public bool IsModded;
        
    [Notify] private bool isPlaying;

    public bool ShouldDisplayToolTip
    {
        get
        {
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
        FormattedDuration = Duration == TimeSpan.Zero ? "(??:??)" : !milliseconds ? $"({Duration:mm\\:ss})" : $"({Duration:ss\\.ff})";
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
}