using HarmonyLib;
using Vintagestory.GameContent;

[HarmonyPatch(typeof(ItemIngot))]
public class Patch_ItemIngot_BlisterUraniumSteel
{
    [HarmonyPostfix]
    [HarmonyPatch("OnLoaded")]
    public static void OnLoaded_Postfix(ItemIngot __instance)
    {
        // Original flag is true for normal blistersteel
        // Extend it to include your custom ingot
        if (__instance.Variant["metal"] == "blisteruraniumsteel")
        {
            // This mimics the original behavior of isBlisterSteel
            __instance.GetType()
                .GetField("isBlisterSteel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(__instance, true);
        }
    }
}
