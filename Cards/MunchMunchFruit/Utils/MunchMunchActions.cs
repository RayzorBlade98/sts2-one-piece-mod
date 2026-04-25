using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using RayzorBladeOnePiece.Cards.MunchMunchFruit.WapoMetal;

namespace RayzorBladeOnePiece.Cards.MunchMunchFruit.Utils;

public static class MunchMunchActions
{
    public static CardModel CreateWapoMetalCard(Player player, bool isUpgraded = false)
    {
        return CreateWapoMetalCards(player, 1, isUpgraded).Single();
    }

    public static List<CardModel> CreateWapoMetalCards(Player player, int amount = 1, bool isUpgraded = false)
    {
        var possibleCards = ModelDb.CardPool<WapoMetalCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint);
        var cards = CardFactory
            .GetDistinctForCombat(player, possibleCards, amount, player.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (isUpgraded)
        {
            cards.ForEach(card => CardCmd.Upgrade(card));
        }

        return cards;
    }
}