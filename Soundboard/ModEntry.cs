using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Soundboard.Helpers;
using Soundboard.APIs;
using StardewValley.Menus;

namespace Soundboard
{
    
    
    internal sealed class ModEntry : Mod
    {
        internal static string Prefix { get; private set; } = string.Empty;
        
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;

        public static IViewEngine? viewEngine;

        public static Soundboard? Soundboard { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Prefix = $"{ModManifest.UniqueID}";

            Harmony.PatchAll();

            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private static bool UpdateMusic_Prefix()
        {
            return false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            if (viewEngine == null)
            {
                Log.Error("Failed to find StardewUI API. Soundboard will not function without StardewUI installed.");
                return;
            }
            
            viewEngine.RegisterViews($"{Prefix}/Views", "assets/views");
            viewEngine.RegisterSprites($"{Prefix}/Sprites", "assets/sprites");
            
            viewEngine.EnableHotReloadingWithSourceSync();

            Soundboard = new Soundboard();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button is SButton.F2)
            {
                Log.Warn(Game1.activeClickableMenu);
            }
        
            if (e.Button is SButton.F3)
            {
                if (Game1.activeClickableMenu is not null)
                {
                    Game1.activeClickableMenu = null;
                    return;
                }
        
                Soundboard?.OpenSoundboard();
            }
        }
    }
}