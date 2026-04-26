using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.RustRustFruit;

[Pool(typeof(RustRustFruitCardPool))]
public class RustyTouch() : CustomCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded
        ? [HoverTipFactory.FromPower<RustPower>(), HoverTipFactory.FromPower<FrailPower>()]
        : [HoverTipFactory.FromPower<RustPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (cardPlay.Target.Block > 0)
        {
            await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);
        }
        else
        {
            await CommonActions.Apply<RustPower>(choiceContext, cardPlay.Target, this, 1m);
        }

        if (IsUpgraded)
        {
            await CommonActions.Apply<FrailPower>(choiceContext, cardPlay.Target, this, 2m);
        }
    }
}