using System.Text.RegularExpressions;

namespace TpApi;

/// <summary>
/// Petite règle métier, présente surtout pour donner matière au test unitaire de la CI.
/// </summary>
public static class TitreUtils
{
    public static string Normaliser(string? titre)
    {
        if (string.IsNullOrWhiteSpace(titre))
            throw new ArgumentException("Le titre ne peut pas être vide", nameof(titre));

        return Regex.Replace(titre.Trim(), @"\s+", " ");
    }
}
