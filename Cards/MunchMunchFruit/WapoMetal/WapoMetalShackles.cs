using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public class WapoMetalShackles() : CustomCard(0, CardType.Skill, CardRarity.Token, TargetType.AllEnemies)
{
    private const string StrengthLossKey = "StrengthLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(StrengthLossKey, 11M)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<WapoMetalShacklesPower>(enemy, this, DynamicVars[StrengthLossKey].BaseValue);
        }
    }

    protected override void OnUpgrade() => DynamicVars[StrengthLossKey].UpgradeValueBy(6M);
}