using System.Globalization;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace RayzorBladeOnePiece.Utils.DynamicVars;

public class CustomCalculatedVar(string name) : CalculatedVar(name)
{
    private Func<PowerModel, Creature?, decimal>? _powerCalc;

    public decimal CalculateOverride(Creature? target)
    {
        if (_owner is CardModel)
        {
            return Calculate(target);
        }

        var multiplier = CalculateMultiplier(target);
        return GetBaseVar().BaseValue + GetExtraVar().BaseValue * multiplier;
    }

    public CalculatedVar WithMultiplier(Func<PowerModel, Creature?, decimal> multiplierCalc)
    {
        if (_powerCalc != null)
        {
            throw new InvalidOperationException(
                $"Tried to set multiplier calc for power on CustomCalculatedVar {Name} twice!");
        }

        _powerCalc = multiplierCalc.Target is not AbstractModel
            ? multiplierCalc
            : throw new InvalidOperationException("Multiplier calc must be static!");

        return this;
    }

    protected override DynamicVar GetBaseVar() => _owner!.GetDynamicVar(Name + "Base");

    protected override DynamicVar GetExtraVar() => _owner!.GetDynamicVar(Name + "Extra");

    protected override decimal GetBaseValueForIConvertible() => CalculateOverride(null);

    public override string ToString() => CalculateOverride(null).ToString(CultureInfo.InvariantCulture);

    private decimal CalculateMultiplier(Creature? target)
    {
        switch (_owner)
        {
            case PowerModel powerModel:
            {
                if (_powerCalc == null)
                {
                    throw new InvalidOperationException(
                        $"CustomCalculatedVar {Name} does not have multiplier calc defined for powers in {_owner.Id}");
                }

                return _powerCalc(powerModel, target);
            }
            default:
                throw new NotImplementedException();
        }
    }
}

public class CustomCalculatedDisplayVar<T>(string name, Func<T, decimal, string> display)
    : DisplayVar<T>($"{name}Display", model =>
    {
        var calculatedVar = (CustomCalculatedVar)model.GetDynamicVar(name);
        return display(model, calculatedVar.CalculateOverride(null));
    }) where T : AbstractModel
{
}