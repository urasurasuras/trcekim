using TrCekim.Core;

namespace TrCekim.Tests;

public class AccessCodesTests
{
    [Fact]
    public void Generate_uses_only_unambiguous_alphabet()
    {
        for (var i = 0; i < 1000; i++)
        {
            var code = AccessCodes.Generate();
            Assert.Equal(AccessCodes.Length, code.Length);
            Assert.All(code, c => Assert.Contains(c, AccessCodes.Alphabet));
        }
        Assert.DoesNotContain('0', AccessCodes.Alphabet);
        Assert.DoesNotContain('O', AccessCodes.Alphabet);
        Assert.DoesNotContain('1', AccessCodes.Alphabet);
        Assert.DoesNotContain('I', AccessCodes.Alphabet);
    }

    [Fact]
    public void Generate_is_not_sequential()
    {
        var codes = Enumerable.Range(0, 500).Select(_ => AccessCodes.Generate()).ToHashSet();
        Assert.Equal(500, codes.Count);
    }

    [Theory]
    [InlineData("abcd-efgh", "ABCDEFGH")]
    [InlineData(" ABCD EFGH ", "ABCDEFGH")]
    [InlineData("abcd–efgh", "ABCDEFGH")] // en dash from phone keyboards
    [InlineData("144001", "144001")] // legacy numeric codes still normalize
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Normalize(string? input, string expected) => Assert.Equal(expected, AccessCodes.Normalize(input));

    [Fact]
    public void Normalize_is_culture_independent_for_turkish_i()
    {
        var previous = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            Assert.Equal("ABIC", AccessCodes.Normalize("abic"));
            Assert.Equal("ABIC", AccessCodes.Normalize("abıc"));
            Assert.Equal("ABIC", AccessCodes.Normalize("abİc"));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Format_inserts_separator() => Assert.Equal("ABCD-EFGH", AccessCodes.Format("ABCDEFGH"));
}

public class PhotoNumbersTests
{
    [Theory]
    [InlineData("017988.JPG", "017988")]
    [InlineData("DSC_0042.jpeg", "DSC_0042")]
    [InlineData("folder/017988.jpg", "017988")]
    [InlineData("  IMG 1.png", "IMG 1")]
    public void FromFileName(string fileName, string expected) => Assert.Equal(expected, PhotoNumbers.FromFileName(fileName));

    [Fact]
    public void OrderReference_has_prefix_and_length()
    {
        var r = OrderReferences.Generate();
        Assert.StartsWith("TRC-", r);
        Assert.Equal(10, r.Length);
    }
}

public class GalleryTests
{
    private static Gallery Make(bool published, DateTimeOffset? expires) => new()
    {
        Season = "2025-2026", Title = "Mezuniyet", AccessCode = "ABCDEFGH", IsPublished = published, ExpiresAt = expires,
    };

    [Fact]
    public void IsOpen_requires_published_and_not_expired()
    {
        var now = DateTimeOffset.UtcNow;
        Assert.True(Make(true, null).IsOpen(now));
        Assert.True(Make(true, now.AddDays(1)).IsOpen(now));
        Assert.False(Make(true, now.AddSeconds(-1)).IsOpen(now));
        Assert.False(Make(false, null).IsOpen(now));
    }
}
