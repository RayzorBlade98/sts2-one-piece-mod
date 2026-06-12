using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RayzorBlade.Sts2.BaseLib.Models;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SpringSpringFruit;

[Pool(typeof(SpringSpringFruitCardPool))]
public class SpringHopper() : ModdedCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SpringedUpPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var count = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            ++count;
        }

        await CommonActions.ApplySelf<SpringedUpPower>(choiceContext, this, count);
    }
}