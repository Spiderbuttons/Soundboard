using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PropertyChanged.SourceGenerator;
using Soundboard.Helpers;
using StardewValley;

namespace Soundboard;

public partial class SoundList
{
    public string Category;

    public List<Sound> Sounds = new List<Sound>();

    public List<Sound> PlayingSounds = new List<Sound>();

    [Notify] public int pageNumber;

    public List<Sound> CurrentPage => Sounds.GetRange(PageNumber * 10, Math.Min(10, Sounds.Count - PageNumber * 10));
    
    public bool AtMaxPage => PageNumber + 1 > Sounds.Count / 10;
    
    public bool AtMinPage => PageNumber <= 0;
    
    public SoundList(string category)
    {
        Category = category;
    }
    
    public void AddSound(Sound sound)
    {
        Sounds.Add(sound);
    }
    
    public void SwitchPage(int direction)
    {
        if (direction > 0)
            NextPage();
        else
            PrevPage();
    }

    public void PrevPage()
    {
        if (AtMinPage) return;
        
        PageNumber = Math.Max(0, PageNumber - 1);
        Game1.playSound("shwip");
    }

    public void NextPage()
    {
        if (AtMaxPage) return;
        
        PageNumber = Math.Min(Sounds.Count / 10, PageNumber + 1);
        Game1.playSound("shwip");
    }

    public void ToggleSound(string id)
    {
        var sound = CurrentPage.FirstOrDefault(s => s.Id == id);
        if (sound == null) return;
        
        if (sound.IsPlaying)
        {
            sound.Stop();
            PlayingSounds.Remove(sound);
        }
        else
        {
            sound.Play();
            if (!PlayingSounds.Contains(sound))
                PlayingSounds.Add(sound);
        }
    }
    
    public void PlaySound(string id)
    {
        var sound = CurrentPage.FirstOrDefault(s => s.Id == id);
        if (sound != null)
        {
            sound.Play();
            if (!PlayingSounds.Contains(sound))
                PlayingSounds.Add(sound);
        }
    }
    
    public void StopSound(string id)
    {
        var sound = PlayingSounds.Find(s => s.Id == id);
        if (sound != null)
        {
            sound.Stop();
            PlayingSounds.Remove(sound);
        }
    }
    
    public void StopAllSounds() 
    {
        foreach (var sound in PlayingSounds)
        {
            sound.Stop();
        }
        PlayingSounds.Clear();
    }

    public void Update()
    {
        foreach (var sound in PlayingSounds)
        {
            // sound.IsPlaying = sound.IsCuePlaying();
        }
    }
}