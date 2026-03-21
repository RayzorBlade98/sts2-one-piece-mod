using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using RayzorBladeOnePiece.Cards.SlowSlowFruit;

namespace RayzorBladeOnePiece.Relics;

public class SlowSlowFruit : DevilFruitRelic<SlowSlowFruitCardPool>
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SlowBeam>()];

    protected override IEnumerable<CardModel> PickUpCardRewards => [Owner.RunState.CreateCard<SlowBeam>(Owner)];
}