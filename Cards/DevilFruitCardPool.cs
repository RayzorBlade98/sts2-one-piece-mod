using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Cards;

public abstract class DevilFruitCardPool : CustomCardPoolModel
{
    public override bool IsColorless => false;
    public override bool IsShared => true;

    public override string BigEnergyIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigEnergyIconPath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToBigEnergyIconPath();
        }
    }

    public override string TextEnergyIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_text.png".ToBigEnergyIconPath();
            return ResourceLoader.Exists(path) ? path : "placeholder_text.png".ToBigEnergyIconPath();
        }
    }
}