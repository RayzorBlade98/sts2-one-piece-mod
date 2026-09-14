using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SpringSpringFruit;

[Pool(typeof(SpringSpringFruitCardPool))]
public class SpringUp() : ModdedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SpringedUpPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SpringedUpPower>(1M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpringedUpPower>(choiceContext, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}