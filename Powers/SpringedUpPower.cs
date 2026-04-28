using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RayzorBladeOnePiece.Utils.DynamicVars;
using CustomCalculatedVar = RayzorBladeOnePiece.Utils.DynamicVars.CustomCalculatedVar;

namespace RayzorBladeOnePiece.Powers;

public class SpringedUpPower : CustomPower
{
    private const string MissRateKey = "MissRate";
    private const string DamageIncreaseKey = "DamageIncrease";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CustomCalculatedVar(MissRateKey).WithMultiplier((power, _) => power.Amount),
        new($"{MissRateKey}Base", 0M),
        new($"{MissRateKey}Extra", 0.1M),
        new CustomCalculatedDisplayVar<SpringedUpPower>(MissRateKey, (_, value) => ((int)(value * 100)).ToString()),
        new CustomCalculatedVar(DamageIncreaseKey).WithMultiplier((power, _) => power.Amount),
        new($"{DamageIncreaseKey}Base", 0M),
        new($"{DamageIncreaseKey}Extra", 0.5M),
        new CustomCalculatedDisplayVar<SpringedUpPower>(DamageIncreaseKey,
            (_, value) => ((int)(value * 100)).ToString())
    ];

    protected override object InitInternalData() => new Data();

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Type != CardType.Attack || card.Owner != Owner.Player)
        {
            return playCount;
        }

        var missRate = ((CustomCalculatedVar)DynamicVars[MissRateKey]).CalculateOverride(null);
        var isMissing = (decimal)Owner.Player.RunState.Rng.Niche.NextDouble() < missRate;
        GetInternalData<Data>().ModifiedCard = card;

        if (!isMissing)
        {
            return playCount;
        }

        Flash();
        return 0;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (card != GetInternalData<Data>().ModifiedCard)
        {
            return;
        }

        await PowerCmd.Remove<SpringedUpPower>(card.Owner.Creature);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != GetInternalData<Data>().ModifiedCard)
        {
            return;
        }

        await PowerCmd.Remove<SpringedUpPower>(cardPlay.Card.Owner.Creature);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack_())
        {
            return 1M;
        }

        var damageIncrease = ((CustomCalculatedVar)DynamicVars[DamageIncreaseKey]).CalculateOverride(target);
        return 1M + damageIncrease;
    }

    private class Data
    {
        public CardModel? ModifiedCard;
    }
}