using BaseLib.Abstracts;
using Godot;

namespace RayzorBladeOnePiece.Cards.SlowSlowFruit;

public class SlowSlowFruitCardPool : DevilFruitCardPool
{
    public override string Title => "SlowSlowFruit";

    public override Color DeckEntryCardColor => new("840240");
    public override Color EnergyOutlineColor => new("651565");
}