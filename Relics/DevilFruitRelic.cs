using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace RayzorBladeOnePiece.Relics;

/**
 * Each devil fruit relic has a corresponding <see cref="CardPoolModel"/> <typeparamref name="TCardPool"/> that is added
 * to the card rewards pool and merchant card pool.
 */
[Pool(typeof(SharedRelicPool))]
public abstract class DevilFruitRelic<TCardPool> : CustomRelic where TCardPool : CardPoolModel
{
    #region Additional card pool

    /**
     * Add the <typeparamref name="TCardPool"/> to the card reward options
     */
    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (Owner != player || options.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications))
        {
            return options;
        }

        var allCards = options.GetPossibleCards(player).ToList();
        var newCards = ModelDb.CardPool<TCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(card => !allCards.Contains(card));

        if (options.Flags.HasFlag(CardCreationFlags.NoRarityModification))
        {
            var allowedRarities = allCards.Select(c => c.Rarity).ToHashSet();
            newCards = newCards.Where(c => allowedRarities.Contains(c.Rarity));
        }

        return options.WithCustomPool(allCards.Concat(newCards));
    }

    /**
     * Add the <typeparamref name="TCardPool"/> to the merchant card pool
     */
    public override IEnumerable<CardModel> ModifyMerchantCardPool(Player player, IEnumerable<CardModel> cards)
    {
        if (Owner != player)
        {
            return cards;
        }

        var allCards = cards.ToList();
        if (allCards.All(card => card.Pool.IsColorless))
        {
            return allCards;
        }

        var newCards = ModelDb.CardPool<TCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(card => !allCards.Contains(card));

        return allCards.Concat(newCards);
    }

    #endregion

    #region Card reward on pickup

    /**
     * List of cards to be rewarded to the player when this relic is obtained.
     */
    protected virtual IEnumerable<CardModel> PickUpCardRewards => [];

    /**
     * Add all reward cards to the player's deck
     */
    public override async Task AfterObtained()
    {
        foreach (var card in PickUpCardRewards)
        {
            var result = await CardPileCmd.Add(card, PileType.Deck);
            CardCmd.PreviewCardPileAdd(result);
        }
    }

    #endregion
}