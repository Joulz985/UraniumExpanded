namespace UraniumExpanded.ModConfiguration
{
    public class ModConfig
    {
        public static ModConfig Loaded { get; set; } = new ModConfig();

        public bool EnableUraniumTools { get; set; } = true;

        public bool EnableUraniumGlass { get; set; } = true;
    }
}