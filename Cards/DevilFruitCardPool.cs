using BaseLib.Abstracts;

namespace RayzorBladeOnePiece.Cards;

public abstract class DevilFruitCardPool : CustomCardPoolModel
{
    public override bool IsColorless => false;
    public override bool IsShared => true;
}