using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

[Pool(typeof(WapoMetalCardPool))]
public class WapoMetalKnuckles() : CustomCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10M, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await CommonActions
            .CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        var blockAmount = attackCommand.Results.Sum(r => r.TotalDamage + r.OverkillDamage);
        await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3M);
}