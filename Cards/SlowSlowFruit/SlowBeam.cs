using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Apply <see cref="SlowBeamPower"/> to an enemy. Exhaust.
 * <br />
 * <b>Upgrade:</b> Remove exhaust.
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class SlowBeam() : CustomCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlowBeamPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<SlowBeamPower>(choiceContext, cardPlay.Target, this, 1m);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}