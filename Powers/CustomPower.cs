using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Powers;

public abstract class CustomPower : CustomPowerModel
{
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToPowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToBigPowerImagePath();
        }
    }
}