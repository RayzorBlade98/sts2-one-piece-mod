using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit;

[Pool(typeof(MunchMunchFruitCardPool))]
public class BeroCannon() : ModdedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string ExhaustCountKey = "ExhaustCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new(ExhaustCountKey, 1M)
    ];

    protected override bool IsPlayable =>
        PileType.Hand.GetPile(Owner).Cards.Count > DynamicVars[ExhaustCountKey].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pile = PileType.Hand.GetPile(Owner);
        for (var i = 0; i < DynamicVars[ExhaustCountKey].IntValue; i++)
        {
            var card = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
            if (card is null)
            {
                return;
            }

            await CardCmd.Exhaust(choiceContext, card);
        }

        await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5M);
        DynamicVars[ExhaustCountKey].UpgradeValueBy(1M);
    }
}