using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        return
        [
            new HealthBarForecastSegment(Amount, new Color("c466be"), HealthBarForecastDirection.FromRight)
        ];
    }

    /**
     * Damage the owner for the stored damage at the end of its turn, then remove this power
     */
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await ApplyStoredDamage(choiceContext);
    }

    /**
     * Damage the owner for the stored damage, then remove this power
     */
    public async Task ApplyStoredDamage(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Damage(choiceContext, Owner, (decimal) Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.Remove(this);
    }
}