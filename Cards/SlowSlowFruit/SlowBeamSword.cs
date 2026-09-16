using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Apply <see cref="SlowBeamPower"/> and deal 4 damage.
 * <br />
 * If the target already has <see cref="SlowBeamPower"/>, remove it and trigger the stored damage from <see cref="SlowBeamCounterPower"/>.
 * <br />
 * <b>Upgrade:</b> Increase damage by 2.
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class SlowBeamSword() : ModdedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlowBeamPower>(),
        HoverTipFactory.FromPower<SlowBeamCounterPower>()
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        CombatState != null && CombatState.HittableEnemies.Any(e => e.HasPower<SlowBeamPower>());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var isAlreadySlowed = cardPlay.Target.HasPower<SlowBeamPower>();
        if (!isAlreadySlowed)
        {
            await CommonActions.Apply<SlowBeamPower>(choiceContext, cardPlay.Target, this, 1m);
        }

        await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_slash").Execute(choiceContext);

        if (!isAlreadySlowed)
        {
            return;
        }

        await PowerCmd.Remove<SlowBeamPower>(cardPlay.Target);
        var storedDamagePower = cardPlay.Target.GetPower<SlowBeamCounterPower>();
        if (storedDamagePower is not null)
        {
            await storedDamagePower.ApplyStoredDamage(choiceContext);
        }
    }
    
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2M);
}