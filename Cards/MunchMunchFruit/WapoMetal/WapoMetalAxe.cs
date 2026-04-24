using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

public class WapoMetalAxe() : WapoMetalCard(1, CardType.Attack, TargetType.AnyEnemy)
{
    private const string WapoMetalMultiplierKey = "WapoMetalMultiplier";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0M),
        new ExtraDamageVar(1M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CalculateDamageMultiplier),
        new(WapoMetalMultiplierKey, 3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, vfx: "vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    private static decimal CalculateDamageMultiplier(CardModel card, Creature? _)
    {
        return CombatManager.Instance.History.CardPlaysFinished.Aggregate(0M, (sum, entry) =>
        {
            if (entry.CardPlay.Card.Owner != card.Owner)
            {
                return sum;
            }

            if (!card.IsUpgraded)
            {
                return sum + 1;
            }

            var increase = entry.CardPlay.Card.Tags.Contains(WapoMetalTags.WapoMetal)
                ? card.DynamicVars[WapoMetalMultiplierKey].BaseValue
                : 1;
            return sum + increase;
        });
    }
}