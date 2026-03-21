using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Relics;

public abstract class CustomRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToRelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".ToRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder_outline.png".ToRelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToBigRelicImagePath();
        }
    }
}