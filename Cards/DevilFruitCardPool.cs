using RayzorBlade.Sts2.BaseLib.Models;

namespace RayzorBladeOnePiece.Cards;

public abstract class DevilFruitCardPool : ModdedCardPool
{
    public override bool IsColorless => false;
    public override bool IsShared => true;
}