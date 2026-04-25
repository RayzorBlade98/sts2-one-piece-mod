using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public abstract class WapoMetalCard(int baseCost, CardType type, TargetType target)
    : CustomCard(baseCost, type, CardRarity.Token, target)
{
    protected override HashSet<CardTag> CanonicalTags => [MunchMunchTags.WapoMetal];
}