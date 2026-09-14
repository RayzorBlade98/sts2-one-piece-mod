using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using RayzorBladeOnePiece.Extensions;

namespace RayzorBladeOnePiece.Cards;

public abstract class ModdedCard(
    int baseCost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true) : CustomCardModel(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
{
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigCardImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToBigCardImagePath();
        }
    }

    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToCardImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".ToCardImagePath();
        }
    }
}