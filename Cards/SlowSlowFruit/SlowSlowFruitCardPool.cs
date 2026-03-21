using BaseLib.Abstracts;
using Godot;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

public class SlowSlowFruitCardPool : CustomCardPoolModel
{
    public override string Title => "SlowSlowFruit";

    public override Color DeckEntryCardColor => new("840240");
    public override Color EnergyOutlineColor => new("651565");

    public override bool IsColorless => false;
    public override bool IsShared => true;
}