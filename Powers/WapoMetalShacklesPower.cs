using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Powers;

public class WapoMetalShacklesPower : TemporaryStrengthPower, ICustomPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<WapoMetalShackles>();

    protected override bool IsPositive => false;

    public string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToPowerImagePath();
    public string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigPowerImagePath();
}