using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Powers;

namespace RayzorBladeOnePiece.Cards.RustRustFruit;

[Pool(typeof(RustRustFruitCardPool))]
public class RustForm() : CustomCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const string RustAmountKey = "RustAmount";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RustPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new(RustAmountKey, 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RustFormPower>(this, DynamicVars[RustAmountKey].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars[RustAmountKey].UpgradeValueBy(1);
}