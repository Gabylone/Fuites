using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringUtils {
    public static string SanitizeFileName(string input) {
        // 1. Enlever les accents
        string normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (char c in normalized) {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) {
                sb.Append(c);
            }
        }

        string noAccents = sb.ToString().Normalize(NormalizationForm.FormC);

        // 2. Remplacer les espaces par des underscores ou tirets
        string noSpaces = Regex.Replace(noAccents, @"\s+", "_");

        // 3. Supprimer les caractères non valides pour un nom de fichier
        string sanitized = Regex.Replace(noSpaces, @"[^a-zA-Z0-9_\-\.]", "");

        return sanitized;
    }
}