using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBladeOnePiece.Cards.SlowSlowFruit;

namespace RayzorBladeOnePiece.Powers;

/**
 * Deals 10 damage to a random enemy repeated for each stack
 */
public class DelayedBombardmentPower : CustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(DelayedBombardment.BombDamage, ValueProp.Unpowered)];

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side || Owner.Player is null || Owner.CombatState is null || !participants.Contains(Owner))
        {
            return;
        }

        Flash();
        await Cmd.CustomScaledWait(0.2f, 0.4f);

        for (var i = 0; i < Amount; i++)
        {
            var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
            if (target is null)
            {
                continue;
            }

            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(target));
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage, Owner);
        }

        await PowerCmd.Remove(this);
    }
}