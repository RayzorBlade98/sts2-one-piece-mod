using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

namespace RayzorBladeOnePiece.Powers;

public class MunchMunchFormPower : CustomPower
{
    private const string IsUpgradedKey = "IsUpgraded";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BoolVar(IsUpgradedKey, false)];

    public void SetIsUpgraded(bool isUpgraded)
    {
        ((BoolVar)DynamicVars[IsUpgradedKey]).BoolVal = isUpgraded;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var cardToTransform = (await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, null, this))
            .FirstOrDefault();
        if (cardToTransform is null)
        {
            return;
        }

        var createdCard = MunchMunchActions.CreateWapoMetalCard(Owner.Player,
            isUpgraded: ((BoolVar)DynamicVars[IsUpgradedKey]).BoolVal);
        await CardCmd.Transform(cardToTransform, createdCard);
    }
}