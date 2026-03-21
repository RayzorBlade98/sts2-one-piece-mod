namespace RayzorBladeOnePiece.Extensions;

public static class ImagePathExtensions
{
    private static string ImagePath => Path.Join(MainFile.ModId, "images");
    private static string CardPortraitsPath => Path.Join(ImagePath, "card_portraits");
    private static string PowerPath => Path.Join(ImagePath, "powers");

    public static string ToCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, path);
    }

    public static string ToBigCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, "big", path);
    }

    public static string ToPowerImagePath(this string path)
    {
        return Path.Join(PowerPath, path);
    }

    public static string ToBigPowerImagePath(this string path)
    {
        return Path.Join(PowerPath, "big", path);
    }
}