using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Cards;

public abstract class ModdedCardPool : CustomCardPoolModel
{
    public override string BigEnergyIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToEnergyIconPath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToEnergyIconPath();
        }
    }

    public override string TextEnergyIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_text.png".ToEnergyIconPath();
            return ResourceLoader.Exists(path) ? path : "placeholder_text.png".ToEnergyIconPath();
        }
    }
}