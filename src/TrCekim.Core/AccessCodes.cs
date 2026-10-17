using System.Security.Cryptography;
using System.Text;

namespace TrCekim.Core;

/// <summary>
/// Gallery access codes. Legacy codes were short and sequential (144001, 144002, …),
/// so anyone could walk through other classes' galleries. New codes are random.
/// </summary>
public static class AccessCodes
{
    /// <summary>No 0/O, 1/I/L: codes are read off paper slips and typed by parents.</summary>
    public const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public const int Length = 8;

    /// <summary>A new random code, normalized (no separator).</summary>
    public static string Generate()
    {
        Span<char> chars = stackalloc char[Length];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }
        return new string(chars);
    }

    /// <summary>
    /// Canonical form for storage and lookup: separators and whitespace removed,
    /// upper-cased with invariant rules (avoids the Turkish i/İ casing trap).
    /// </summary>
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c switch
                {
                    // A Turkish keyboard may produce these for i/I.
                    'ı' or 'i' or 'İ' => 'I',
                    _ => char.ToUpperInvariant(c),
                });
            }
        }
        return sb.ToString();
    }

    /// <summary>Human-friendly form for slips: "ABCD-EFGH".</summary>
    public static string Format(string code) =>
        code.Length == Length ? $"{code[..4]}-{code[4..]}" : code;
}
