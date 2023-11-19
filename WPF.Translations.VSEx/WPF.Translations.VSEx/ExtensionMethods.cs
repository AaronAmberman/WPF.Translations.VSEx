namespace WPF.Translations.VSEx
{
    public static class ExtensionMethods
    {
        public static string GetCultureFromFileName(this string fileName, string fileExtension) 
        {
            if (fileName.Contains("Translations")) // English
                return fileName.Replace("Translations.", "").Replace(fileExtension, "");
            else if (fileName.Contains("translations")) // English
                return fileName.Replace("translations.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Übersetzung")) // German
                return fileName.Replace("Übersetzung.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Traducción")) // Spanish
                return fileName.Replace("Traducción.", "").Replace(fileExtension, "");
            else if (fileName.Contains("traducción")) // Spanish
                return fileName.Replace("traducción.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Traduction")) // French
                return fileName.Replace("Traduction.", "").Replace(fileExtension, "");
            else if (fileName.Contains("traduction")) // French
                return fileName.Replace("traduction.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Traduzione")) // Italian
                return fileName.Replace("Traduzione.", "").Replace(fileExtension, "");
            else if (fileName.Contains("traduzione")) // Italian
                return fileName.Replace("traduzione.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Oversettelse")) // Norwegian
                return fileName.Replace("Oversettelse.", "").Replace(fileExtension, "");
            else if (fileName.Contains("oversettelse")) // Norwegian
                return fileName.Replace("oversettelse.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Vertaling")) // Dutch
                return fileName.Replace("Vertaling.", "").Replace(fileExtension, "");
            else if (fileName.Contains("vertaling")) // Dutch
                return fileName.Replace("vertaling.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Käännös")) // Finish
                return fileName.Replace("Käännös.", "").Replace(fileExtension, "");
            else if (fileName.Contains("käännös")) // Finish
                return fileName.Replace("käännös.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Tradução")) // Portuguese
                return fileName.Replace("Tradução.", "").Replace(fileExtension, "");
            else if (fileName.Contains("tradução")) // Portuguese
                return fileName.Replace("tradução.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Fordítás")) // Hungarian
                return fileName.Replace("Fordítás.", "").Replace(fileExtension, "");
            else if (fileName.Contains("fordítás")) // Hungarian
                return fileName.Replace("fordítás.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Traducere")) // Romanian
                return fileName.Replace("Traducere.", "").Replace(fileExtension, "");
            else if (fileName.Contains("traducere")) // Romanian
                return fileName.Replace("traducere.", "").Replace(fileExtension, "");
            else if (fileName.Contains("перевод")) // Russian
                return fileName.Replace("перевод.", "").Replace(fileExtension, "");
            else if (fileName.Contains("превод")) // Bulgarian
                return fileName.Replace("превод.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Oversættelse")) // Danish
                return fileName.Replace("Oversættelse.", "").Replace(fileExtension, "");
            else if (fileName.Contains("oversættelse")) // Danish
                return fileName.Replace("oversættelse.", "").Replace(fileExtension, "");
            else if (fileName.Contains("μετάφραση")) // Greek
                return fileName.Replace("μετάφραση.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Tłumaczenie")) // Polish
                return fileName.Replace("Tłumaczenie.", "").Replace(fileExtension, "");
            else if (fileName.Contains("tłumaczenie")) // Polish
                return fileName.Replace("tłumaczenie.", "").Replace(fileExtension, "");
            else if (fileName.Contains("översättning")) // Swedish
                return fileName.Replace("översättning.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Tercüme")) // Turkish
                return fileName.Replace("Tercüme.", "").Replace(fileExtension, "");
            else if (fileName.Contains("tercüme")) // Turkish
                return fileName.Replace("tercüme.", "").Replace(fileExtension, "");
            else if (fileName.Contains("翻译")) // Chinese Simplified
                return fileName.Replace("翻译.", "").Replace(fileExtension, "");
            else if (fileName.Contains("翻譯")) // Chinese Traditional
                return fileName.Replace("翻譯.", "").Replace(fileExtension, "");
            else if (fileName.Contains("翻訳")) // Japanese
                return fileName.Replace("翻訳.", "").Replace(fileExtension, "");
            else if (fileName.Contains("번역")) // Korean
                return fileName.Replace("번역.", "").Replace(fileExtension, "");
            else if (fileName.Contains("การแปล")) // Thai
                return fileName.Replace("การแปล.", "").Replace(fileExtension, "");
            else if (fileName.Contains("Dịch")) // Vietnamese
                return fileName.Replace("Dịch.", "").Replace(fileExtension, "");
            else if (fileName.Contains("dịch")) // Vietnamese
                return fileName.Replace("dịch.", "").Replace(fileExtension, "");

            return string.Empty;
        }

        public static bool IsTranslationsDirectory(this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (path.EndsWith("translations\\", StringComparison.OrdinalIgnoreCase)) return true; // English
            if (path.EndsWith("Übersetzung\\", StringComparison.OrdinalIgnoreCase)) return true; // German
            if (path.EndsWith("traducción\\", StringComparison.OrdinalIgnoreCase)) return true; // Spanish
            if (path.EndsWith("traduction\\", StringComparison.OrdinalIgnoreCase)) return true; // French
            if (path.EndsWith("traduzione\\", StringComparison.OrdinalIgnoreCase)) return true; // Italian
            if (path.EndsWith("oversettelse\\", StringComparison.OrdinalIgnoreCase)) return true; // Norwegian
            if (path.EndsWith("vertaling\\", StringComparison.OrdinalIgnoreCase)) return true; // Dutch
            if (path.EndsWith("käännös\\", StringComparison.OrdinalIgnoreCase)) return true; // Finish
            if (path.EndsWith("tradução\\", StringComparison.OrdinalIgnoreCase)) return true; // Portuguese
            if (path.EndsWith("fordítás\\", StringComparison.OrdinalIgnoreCase)) return true; // Hungarian
            if (path.EndsWith("traducere\\", StringComparison.OrdinalIgnoreCase)) return true; // Romanian
            if (path.EndsWith("перевод\\", StringComparison.OrdinalIgnoreCase)) return true; // Russian
            if (path.EndsWith("превод\\", StringComparison.OrdinalIgnoreCase)) return true; // Bulgarian
            if (path.EndsWith("oversættelse\\", StringComparison.OrdinalIgnoreCase)) return true; // Danish
            if (path.EndsWith("μετάφραση\\", StringComparison.OrdinalIgnoreCase)) return true; // Greek
            if (path.EndsWith("tłumaczenie\\", StringComparison.OrdinalIgnoreCase)) return true; // Polish
            if (path.EndsWith("översättning\\", StringComparison.OrdinalIgnoreCase)) return true; // Swedish
            if (path.EndsWith("tercüme\\", StringComparison.OrdinalIgnoreCase)) return true; // Turkish
            if (path.EndsWith("翻译\\", StringComparison.OrdinalIgnoreCase)) return true; // Chinese Simplified
            if (path.EndsWith("翻譯\\", StringComparison.OrdinalIgnoreCase)) return true; // Chinese Traditional
            if (path.EndsWith("翻訳\\", StringComparison.OrdinalIgnoreCase)) return true; // Japanese
            if (path.EndsWith("번역\\", StringComparison.OrdinalIgnoreCase)) return true; // Korean
            if (path.EndsWith("การแปล\\", StringComparison.OrdinalIgnoreCase)) return true; // Thai
            if (path.EndsWith("dịch\\", StringComparison.OrdinalIgnoreCase)) return true; // Vietnamese

            return false;
        }
    }
}
