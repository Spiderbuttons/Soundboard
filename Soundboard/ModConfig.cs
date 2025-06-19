using StardewModdingAPI;

namespace Soundboard;

public class ModConfig
{
    public SButton Keybind { get; set; } = SButton.F3;
    public bool ForceTooltips { get; set; } = false;
    public bool VanillaOnly { get; set; } = false;
}