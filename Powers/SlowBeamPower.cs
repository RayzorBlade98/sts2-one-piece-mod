using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Powers;

/**
 * Debuff that reduces the owners damage to <see cref="DamageDecreaseKey"/> % (75%) for one turn.
 * <br />
 * If the user receives card damage, it's reduced to zero and instead <see cref="StoredDamageIncreaseKey"/> % (150%)
 * of the damage is stored as <see cref="SlowBeamCounterPower"/>
 *
 * <remarks>
 * Currently only works for enemy creatures as it checks for card sources
 * </remarks>
 */
public class SlowBeamPower : CustomPower
{
    private const string DamageDecreaseKey = "DamageDecrease";
    private const string StoredDamageIncreaseKey = "StoredDamageIncrease";

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(DamageDecreaseKey, 0.75M), new(StoredDamageIncreaseKey, 1.5M)];

    /**
     * Reduce the damage dealt by the owner to <see cref="DamageDecreaseKey"/> %
     */
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return dealer != Owner || !props.IsPoweredAttack_() ? 1M : DynamicVars[DamageDecreaseKey].BaseValue;
    }

    /**
     * If the owner receives card damage, reduce it to zero and instead apply <see cref="StoredDamageIncreaseKey"/> %
     * of it as <see cref="SlowBeamCounterPower"/>.
     */
    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || cardSource is null || amount <= 0M)
        {
            return amount;
        }
        
        var storedDamage = amount * DynamicVars[StoredDamageIncreaseKey].BaseValue;
        CommonActions.Apply<SlowBeamCounterPower>(new ThrowingPlayerChoiceContext(), target, null, storedDamage);
        
        // Handle slippery power (decrement slippery after storing damage)
        var slipperyPower = Owner.GetPower<SlipperyPower>();
        if (slipperyPower is not null)
        {
            PowerCmd.Decrement(slipperyPower);
        }
        
        return 0M;
    }

    /**
     * Remove power after enemy turn
     */
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}