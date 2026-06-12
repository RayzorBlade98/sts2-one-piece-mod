using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBlade.Sts2.BaseLib.Models;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Deals 2 damage 5 times. If the target has <see cref="SlowBeamPower"/> it hits 9 times instead.
 * <br />
 * <b>Upgrade:</b> Increase damage by 1.
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class KyubiRush() : ModdedCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string RepeatOnSlowedKey = "RepeatOnSlowed";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2M, ValueProp.Move),
        new RepeatVar(5),
        new(RepeatOnSlowedKey, 9m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlowBeamPower>()];

    protected override bool ShouldGlowGoldInternal =>
        CombatState != null && CombatState.HittableEnemies.Any(e => e.HasPower<SlowBeamPower>());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hitCount = cardPlay.Target.HasPower<SlowBeamPower>()
            ? DynamicVars[RepeatOnSlowedKey].IntValue
            : DynamicVars.Repeat.IntValue;
        await CommonActions.CardAttack(this, cardPlay, hitCount, "vfx/vfx_attack_blunt").Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1M);
}