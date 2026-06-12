using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using RayzorBlade.Sts2.BaseLib.Models;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public abstract class WapoMetalCard(int baseCost, CardType type, TargetType target)
    : ModdedCard(baseCost, type, CardRarity.Token, target)
{
    protected override HashSet<CardTag> CanonicalTags => [MunchMunchTags.WapoMetal];
}