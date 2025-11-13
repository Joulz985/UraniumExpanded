namespace UraniumExpanded.ModConfiguration
{
    public class ModConfig
    {
        public static ModConfig Loaded { get; set; } = new ModConfig();

        public bool EnableUraniumTools { get; set; } = false;

        public bool EnableUraniumGlass { get; set; } = false;
    }
}