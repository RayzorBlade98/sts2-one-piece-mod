using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace RayzorBladeOnePiece.Powers;

/**
 * Counter debuff applied by <see cref="SlowBeamPower"/> that damages the owner for the stored damage at the end of the turn,
 * then is removed.
 */
public class SlowBeamCounterPower : CustomPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /**
     * Damage the owner for the stored damage at the end of its turn, then remove this power
     */
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Applier,
            null);
        await PowerCmd.Remove(this);
    }
}