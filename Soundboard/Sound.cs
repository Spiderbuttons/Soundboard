using System;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;

namespace Soundboard;

public partial class Sound
{
    private readonly Cue _cue;
    public string Id;

    public TimeSpan Duration;

    public string FormattedDuration;

    public bool DoesLoop;

    public long LoopCount;

    public bool IsModded;
        
    [Notify] public bool isPlaying;

    public string Transform => IsPlaying ? "scale: 0.95" : "scale: 1";
        
    public Sound(string id, Cue cue, bool milliseconds = false)
    {
        Id = id;
        _cue = cue;
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
    }

    public void ToggleState()
    {
        if (_cue.IsPlaying == true) Stop();
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
        IsPlaying = IsCuePlaying();
    }
}