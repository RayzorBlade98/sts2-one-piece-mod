using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SpringSpringFruit;

[Pool(typeof(SpringSpringFruitCardPool))]
public class SpringDeathKnock() : CustomCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SpringedUpPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(27M),
        new ExtraDamageVar(3M),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => card.Owner.Creature.GetPower<SpringedUpPower>()?.Amount ?? 0M)
    ];

    protected override bool ShouldGlowGoldInternal => CombatState != null && Owner.HasPower<SpringedUpPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt").Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.ExtraDamage.UpgradeValueBy(2M);
}