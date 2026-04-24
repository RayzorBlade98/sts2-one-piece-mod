using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public class WapoMetalBomb() : CustomCard(2, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    private const string TurnsKey = "Turns";
    private const string BombDamageKey = "BombDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, 3M),
        new(BombDamageKey, 50M)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bombPower = await CommonActions.ApplySelf<WapoMetalBombPower>(this, DynamicVars[TurnsKey].BaseValue);
        bombPower?.SetDamage(DynamicVars[BombDamageKey].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars[BombDamageKey].UpgradeValueBy(10M);
}