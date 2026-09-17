using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace RayzorBladeOnePiece.Relics.Ancients.Neow;

[Pool(typeof(EventRelicPool))]
public class DealWithTheDevil : CustomRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override bool IsAllowedAtNeow(Player player) => player.Relics.Any(r => r.Rarity == RelicRarity.Starter);

    public override async Task AfterObtained()
    {
        var starter = Owner.Relics.FirstOrDefault(r => r.Rarity == RelicRarity.Starter);
        var fruit = RollRandomFruit(Owner);
        if (starter is null)
        {
            await RelicCmd.Obtain(fruit, Owner);
            return;
        }

        await RelicCmd.Replace(starter, fruit);
    }

    private static IEnumerable<DevilFruitRelic> GetEligibleFruits(Player player) =>
        ModelDb.AllRelics.OfType<DevilFruitRelic>().Where(r => r.IsAllowed(player.RunState));

    private static DevilFruitRelic RollRandomFruit(Player player)
    {
        var candidates = GetEligibleFruits(player)
            .Where(r => player.GetRelicById(r.Id) is null)
            .ToList();
        if (candidates.Count == 0)
        {
            candidates = GetEligibleFruits(player).ToList();
        }

        player.PlayerRng.Rewards.Shuffle(candidates);
        return (DevilFruitRelic)candidates[0].ToMutable();
    }
}