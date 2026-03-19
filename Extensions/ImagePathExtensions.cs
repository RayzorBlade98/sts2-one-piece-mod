namespace RayzorBladeOnePiece.Extensions;

public static class ImagePathExtensions
{
    private static string ImagePath => Path.Join(MainFile.ModId, "images");
    private static string CardPortraitsPath => Path.Join(ImagePath, "card_portraits");

    public static string ToCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, path);
    }

    public static string ToBigCardImagePath(this string path)
    {
        return Path.Join(CardPortraitsPath, "big", path);
    }
}