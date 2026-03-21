using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace RayzorBladeOnePiece.Extensions;

public static class CreatureExtensions
{
    public static CombatSide GetCombatSide(this Creature creature)
    {
        return creature.IsEnemy ? CombatSide.Enemy : CombatSide.Player;
    }
}