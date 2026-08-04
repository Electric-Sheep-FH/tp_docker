using TpApi;
using Xunit;

public class TitreUtilsTests
{
    [Fact]
    public void Normalise_Les_Espaces()
    {
        Assert.Equal("faire le TP", TitreUtils.Normaliser("  faire   le TP  "));
    }

    [Fact]
    public void Refuse_Un_Titre_Vide()
    {
        Assert.Throws<ArgumentException>(() => TitreUtils.Normaliser("   "));
    }
}
