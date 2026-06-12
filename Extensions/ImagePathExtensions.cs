namespace RayzorBladeOnePiece.Extensions;

public static class ImagePathExtensions
{
    private static string ImagePath => Path.Join(MainFile.ModId, "images");
    private static string PowerPath => Path.Join(ImagePath, "powers");
    private static string RelicPath => Path.Join(ImagePath, "relics");

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