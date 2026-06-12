using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBlade.Sts2.BaseLib.Models;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit;

[Pool(typeof(MunchMunchFruitCardPool))]
public class MunchMunchForm() : ModdedCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MunchMunchFormPower>(1M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<MunchMunchFormPower>(choiceContext, this);
        power?.SetIsUpgraded(IsUpgraded);
    }
}