using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using RayzorBladeOnePiece.Cards.MunchMunchFruit;

namespace RayzorBladeOnePiece.Relics;

public class MunchMunchFruit : DevilFruitRelic<MunchMunchFruitCardPool>
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Munch>()];

    protected override IEnumerable<CardModel> PickUpCardRewards =>
    [
        Owner.RunState.CreateCard<Munch>(Owner),
        Owner.RunState.CreateCard<Munch>(Owner)
    ];
}
