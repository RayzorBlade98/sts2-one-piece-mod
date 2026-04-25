using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit;

[Pool(typeof(MunchMunchFruitCardPool))]
public class MunchMunchForm() : CustomCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MunchMunchFormPower>(1M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<MunchMunchFormPower>(this,
            DynamicVars[nameof(MunchMunchFormPower)].BaseValue);
        power?.SetIsUpgraded(IsUpgraded);
    }
}