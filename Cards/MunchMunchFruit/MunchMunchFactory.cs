using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit;

[Pool(typeof(MunchMunchFruitCardPool))]
public class MunchMunchFactory() : ModdedCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var cardToTransform = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        if (cardToTransform is null)
        {
            return;
        }

        var createdCard = MunchMunchActions.CreateWapoMetalCard(Owner, isUpgraded: IsUpgraded);
        await CardCmd.Transform(cardToTransform, createdCard);
    }
}