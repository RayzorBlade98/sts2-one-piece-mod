using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

public class WapoMetalBolas() : WapoMetalCard(0, CardType.Attack, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4M, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || Pile is { Type: PileType.Hand })
        {
            return;
        }

        if (CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.HappenedLastPlayerTurn(Owner) && e.CardPlay.Card == this))
        {
            await CardPileCmd.Add(this, PileType.Hand);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1M);
}