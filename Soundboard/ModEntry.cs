using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GenericModConfigMenu;
using HarmonyLib;
using Ionic.Zlib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Soundboard.Helpers;
using Soundboard.APIs;
using StardewModdingAPI.Framework.Models;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Soundboard
{
    
    
    internal sealed class ModEntry : Mod
    {
        internal static string Prefix { get; private set; } = string.Empty;
        
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;
        internal static ModConfig ModConfig { get; set; } = null!;

        public static IViewEngine? viewEngine;

        public static SoundBank VanillaSoundBank = null!;

        public static Soundboard? Soundboard { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            ModConfig = helper.ReadConfig<ModConfig>();
            Harmony = new Harmony(ModManifest.UniqueID);
            Prefix = $"{ModManifest.UniqueID}";
            
            string rootpath = Game1.content.RootDirectory;
            AudioEngine obj = new AudioEngine(Path.Combine(rootpath, "XACT", "FarmerSounds.xgs"));
            var waveBank = new WaveBank(obj, Path.Combine(rootpath, "XACT", "Wave Bank.xwb"));
            var waveBank1_4 = new WaveBank(obj, Path.Combine(rootpath, "XACT", "Wave Bank(1.4).xwb"));
            VanillaSoundBank = new SoundBank(obj, Path.Combine(rootpath, "XACT", "Sound Bank.xsb"));

            // VanillaSoundBank = new SoundBank(new AudioEngine(Path.Combine(Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")),
            //     Path.Combine(Game1.content.RootDirectory, "XACT", "Sound Bank.xsb"));

            Harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForMusic)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ReusablePrefix))
            );

            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;
            Helper.Events.Content.AssetReady += OnAssetReady;
        }

        private static bool ReusablePrefix()
        {
            return Soundboard?.IsOpen != true;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
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
            
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            gmcm?.Register(
                mod: ModManifest,
                reset: () => ModConfig = new ModConfig(),
                save: () => Helper.WriteConfig(ModConfig),
                titleScreenOnly: false
            );
            gmcm?.AddKeybind(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.Keybind"),
                getValue: () => ModConfig.Keybind,
                setValue: value => ModConfig.Keybind = value
            );
            gmcm?.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.Tooltips"),
                tooltip: () => Helper.Translation.Get("Config.Tooltips.Desc"),
                getValue: () => ModConfig.ForceTooltips,
                setValue: value => ModConfig.ForceTooltips = value
            );
            gmcm?.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.Vanilla"),
                tooltip: () => Helper.Translation.Get("Config.Vanilla.Desc"),
                getValue: () => ModConfig.VanillaOnly,
                setValue: value =>
                {
                    ModConfig.VanillaOnly = value;
                    if (Soundboard is not null) Soundboard = new Soundboard();
                });
            
            viewEngine.RegisterViews($"{Prefix}/Views", "assets/views");
            viewEngine.RegisterSprites($"{Prefix}/Sprites", "assets/sprites");
            
            viewEngine.EnableHotReloadingWithSourceSync();
            
            viewEngine.PreloadAssets();
            viewEngine.PreloadModels(typeof(Soundboard), typeof(SoundList), typeof(Sound));
        }
        
        private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo("Data/AudioChanges")))
            {
                Soundboard._cueChanges = null;
            }
        }

        private void OnAssetReady(object? sender, AssetReadyEventArgs e)
        {
            if (ModConfig.VanillaOnly) return;
            
            if (e.NameWithoutLocale.IsEquivalentTo("Data/AudioChanges"))
            {
                Soundboard?.GetCues(modifiedOnly: true);
            }
        }
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || e.Button != ModConfig.Keybind)
                return;
            
            if (Game1.activeClickableMenu is not null)
            {
                Soundboard?.CloseSoundboard();
                return;
            }
        
            Soundboard?.OpenSoundboard();
        }
    }
}