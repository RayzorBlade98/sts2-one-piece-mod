using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RayzorBlade.Sts2.BaseLib.Models;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Apply <see cref="SlowBeamPower"/> to all enemies
 * <br />
 * <b>Upgrade:</b> Add retain.
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class DoubleSlowBeam() : ModdedCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlowBeamPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return;
        }

        foreach (var target in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<SlowBeamPower>(choiceContext, target, this, 1m);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}