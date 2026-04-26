using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Apply <see cref="DelayedBombardmentPower"/> that damages random enemies for 10 damage 3 times at the end of their turn.
 * <br />
 * <b>Upgrade:</b> Increase the number of times the bomb hits by 1
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class DelayedBombardment() : CustomCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public const decimal BombDamage = 10m;
    private const string BombDamageKey = "BombDamage";
    private const string BombAmountKey = "BombAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(BombAmountKey, 3m),
        new(BombDamageKey, BombDamage)
    ];


    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DelayedBombardmentPower>(context, this, DynamicVars[BombAmountKey].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars[BombAmountKey].UpgradeValueBy(1m);
}