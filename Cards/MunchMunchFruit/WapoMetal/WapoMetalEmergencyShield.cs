using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

public class WapoMetalEmergencyShield() : WapoMetalCard(0, CardType.Skill, TargetType.Self)
{
    private const string TurnsKey = "Turns";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(40M, ValueProp.Move),
        new(TurnsKey, 2M)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<NoBlockPower>(this, DynamicVars[TurnsKey].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(10M);
}