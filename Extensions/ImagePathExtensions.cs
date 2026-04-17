namespace RayzorBladeOnePiece.Extensions;

public static class ImagePathExtensions
{
    private static string ImagePath => Path.Join(MainFile.ModId, "images");
    private static string CardPortraitsPath => Path.Join(ImagePath, "card_portraits");
    private static string EnergyPath => Path.Join(ImagePath, "energy");
    private static string PowerPath => Path.Join(ImagePath, "powers");
    private static string RelicPath => Path.Join(ImagePath, "relics");

    public static string ToCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, path);
    }

    public static string ToBigCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, "big", path);
    }
    
    public static string ToBigEnergyIconPath(this string path)
    {
        return Path.Join(EnergyPath, path);
    }

    public static string ToPowerImagePath(this string path)
    {
        return Path.Join(PowerPath, path);
    }

    public static string ToBigPowerImagePath(this string path)
    {
        return Path.Join(PowerPath, "big", path);
    }

    public static string ToRelicImagePath(this string path)
    {
        return Path.Join(RelicPath, path);
    }

    public static string ToBigRelicImagePath(this string path)
    {
        return Path.Join(RelicPath, "big", path);
    }
}