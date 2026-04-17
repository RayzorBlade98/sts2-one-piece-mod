using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public class WapoMetalShield() : CustomCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15M, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5M);
}