namespace TrCekim.Core;

public static class PhotoNumbers
{
    /// <summary>
    /// Photo number parents see and quote, taken from the camera file name:
    /// "017988.JPG" → "017988", "DSC_0042.jpg" → "DSC_0042".
    /// </summary>
    public static string FromFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(Path.GetFileName(fileName)).Trim();
        return name.Length > 64 ? name[..64] : name;
    }
}

public static class OrderReferences
{
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    /// <summary>Short order reference such as "TRC-7KQ4M2".</summary>
    public static string Generate()
    {
        Span<char> chars = stackalloc char[6];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = Alphabet[System.Security.Cryptography.RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }
        return "TRC-" + new string(chars);
    }
}
