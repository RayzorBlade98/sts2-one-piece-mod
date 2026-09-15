using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

/**
 * Apply <see cref="NoroNoroFoxyFaceBombPower"/> that damages random enemies for 10 damage 3 times at the end of their turn.
 * <br />
 * <b>Upgrade:</b> Focus enemies with <see cref="SlowBeamPower"/>
 */
[Pool(typeof(SlowSlowFruitCardPool))]
public class NoroNoroFoxyFaceBomb() : ModdedCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public const decimal BombDamage = 10m;
    private const string BombDamageKey = "BombDamage";
    private const string BombAmountKey = "BombAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(BombAmountKey, 3m),
        new(BombDamageKey, BombDamage)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        IsUpgraded ? [HoverTipFactory.FromPower<SlowBeamPower>()] : [];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<NoroNoroFoxyFaceBombPower>(context, this,
            DynamicVars[BombAmountKey].BaseValue);
        power?.Init(DynamicVars[BombDamageKey].BaseValue, IsUpgraded);
    }
}