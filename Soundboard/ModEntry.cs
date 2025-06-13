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
            Harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForMusic)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(checkForMusic_Prefix))
            );

            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;
        }

        private static bool checkForMusic_Prefix()
        {
            return Soundboard?.IsOpen != true;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            foreach (var list in Soundboard.Sounds.Values)
            {
                list.Clear();
            }
            Soundboard = new Soundboard();
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
        }
        
        private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo("Data/AudioChanges")))
            {
                Soundboard._cueChanges = null;
            }
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button is SButton.F2)
            {
                if (Game1.activeClickableMenu is not null)
                {
                    Game1.activeClickableMenu = null;
                    return;
                }
        
                Soundboard?.OpenTestBoard();
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