using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using RayzorBlade.Sts2.BaseLib;

namespace RayzorBladeOnePiece;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "RayzorBladeOnePiece";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfig.ModId = ModId;
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}
