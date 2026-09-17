using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using RayzorBladeOnePiece.Relics;
using RayzorBladeOnePiece.Relics.Ancients.Neow;

namespace RayzorBladeOnePiece.Patches;

[HarmonyPatch(typeof(Neow))]
public static class NeowPatch
{
    [HarmonyPatch("AllPossibleOptions", MethodType.Getter)]
    [HarmonyPostfix]
    public static void AddToAllPossibleOptions(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        __result = __result.Concat(BuildCursedOptions(__instance));
    }
    
    [HarmonyPatch("CurseOptions", MethodType.Getter)]
    [HarmonyPostfix]
    public static void AddToCurseOptions(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        __result = __result.Concat(BuildCursedOptions(__instance));
    }

    private static IEnumerable<EventOption> BuildCursedOptions(Neow neow)
    {
        return [BuildOption<DealWithTheDevil>(neow, customDonePage: "NEOW.pages.DONE.CURSED.description")];
    }
    
    private static EventOption BuildOption<T>(Neow neow, string pageName = "INITIAL", string? customDonePage = null)
        where T : CustomRelic
    {
        var relic = ModelDb.Relic<T>().ToMutable();
        var method = AccessTools.Method(typeof(AncientEventModel), "RelicOption",
            [typeof(RelicModel), typeof(string), typeof(string)]);
        return (EventOption)method.Invoke(neow, [relic, pageName, customDonePage])!;
    }
}