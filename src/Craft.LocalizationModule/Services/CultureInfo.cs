namespace Craft.LocalizationModule.Services;

/// <summary>
///     A static class that contains all the cultures
/// </summary>
public static class CultureInfo
{
    // This is a list of all cultures.
    // Credits: https://gist.github.com/hikalkan/afe23b47c30fea418f607561d277c510

    /// <summary>
    ///     Dictionary of all cultures
    /// </summary>
    public static Dictionary<string, string> AllCultures = new Dictionary<
        string,
        string
    >
    {
        { "af", "Afrikaans" },
        { "af-ZA", "Afrikaans - South Africa" },
        { "ar", "Arabic" },
        { "ar-AE", "Arabic - United Arab Emirates" },
        { "ar-BH", "Arabic - Bahrain" },
        { "ar-DZ", "Arabic - Algeria" },
        { "ar-EG", "Arabic - Egypt" },
        { "ar-IQ", "Arabic - Iraq" },
        { "ar-JO", "Arabic - Jordan" },
        { "ar-KW", "Arabic - Kuwait" },
        { "ar-LB", "Arabic - Lebanon" },
        { "ar-LY", "Arabic - Libya" },
        { "ar-MA", "Arabic - Morocco" },
        { "ar-OM", "Arabic - Oman" },
        { "ar-QA", "Arabic - Qatar" },
        { "ar-SA", "Arabic - Saudi Arabia" },
        { "ar-SY", "Arabic - Syria" },
        { "ar-TN", "Arabic - Tunisia" },
        { "ar-YE", "Arabic - Yemen" },
        { "be", "Belarusian" },
        { "be-BY", "Belarusian - Belarus" },
        { "bg", "Bulgarian" },
        { "bg-BG", "Bulgarian - Bulgaria" },
        { "ca", "Catalan" },
        { "ca-ES", "Catalan - Catalan" },
        { "cs", "Czech" },
        { "cs-CZ", "Czech - Czech Republic" },
        { "Cy-az", "Azeri (Cyrillic)" },
        { "Cy-az-AZ", "Azeri (Cyrillic) - Azerbaijan" },
        { "Cy-sr", "Serbian (Cyrillic)" },
        { "Cy-sr-SP", "Serbian (Cyrillic) - Serbia" },
        { "Cy-uz", "Uzbek (Cyrillic)" },
        { "Cy-uz-UZ", "Uzbek (Cyrillic) - Uzbekistan" },
        { "da", "Danish" },
        { "da-DK", "Danish - Denmark" },
        { "de", "German" },
        { "de-AT", "German - Austria" },
        { "de-CH", "German - Switzerland" },
        { "de-DE", "German - Germany" },
        { "de-LI", "German - Liechtenstein" },
        { "de-LU", "German - Luxembourg" },
        { "div", "Dhivehi" },
        { "div-MV", "Dhivehi - Maldives" },
        { "el", "Greek" },
        { "el-GR", "Greek - Greece" },
        { "en", "English" },
        { "en-AU", "English - Australia" },
        { "en-BZ", "English - Belize" },
        { "en-CA", "English - Canada" },
        { "en-CB", "English - Caribbean" },
        { "en-GB", "English - United Kingdom" },
        { "en-IE", "English - Ireland" },
        { "en-JM", "English - Jamaica" },
        { "en-NZ", "English - New Zealand" },
        { "en-PH", "English - Philippines" },
        { "en-TT", "English - Trinidad and Tobago" },
        { "en-US", "English - United States" },
        { "en-ZA", "English - South Africa" },
        { "en-ZW", "English - Zimbabwe" },
        { "es", "Spanish" },
        { "es-AR", "Spanish - Argentina" },
        { "es-BO", "Spanish - Bolivia" },
        { "es-CL", "Spanish - Chile" },
        { "es-CO", "Spanish - Colombia" },
        { "es-CR", "Spanish - Costa Rica" },
        { "es-DO", "Spanish - Dominican Republic" },
        { "es-EC", "Spanish - Ecuador" },
        { "es-ES", "Spanish - Spain" },
        { "es-GT", "Spanish - Guatemala" },
        { "es-HN", "Spanish - Honduras" },
        { "es-MX", "Spanish - Mexico" },
        { "es-NI", "Spanish - Nicaragua" },
        { "es-PA", "Spanish - Panama" },
        { "es-PE", "Spanish - Peru" },
        { "es-PR", "Spanish - Puerto Rico" },
        { "es-PY", "Spanish - Paraguay" },
        { "es-SV", "Spanish - El Salvador" },
        { "es-UY", "Spanish - Uruguay" },
        { "es-VE", "Spanish - Venezuela" },
        { "et", "Estonian" },
        { "et-EE", "Estonian - Estonia" },
        { "eu", "Basque" },
        { "eu-ES", "Basque - Basque" },
        { "fa", "Farsi" },
        { "fa-IR", "Farsi - Iran" },
        { "fi", "Finnish" },
        { "fi-FI", "Finnish - Finland" },
        { "fo", "Faroese" },
        { "fo-FO", "Faroese - Faroe Islands" },
        { "fr", "French" },
        { "fr-BE", "French - Belgium" },
        { "fr-CA", "French - Canada" },
        { "fr-CH", "French - Switzerland" },
        { "fr-FR", "French - France" },
        { "fr-LU", "French - Luxembourg" },
        { "fr-MC", "French - Monaco" },
        { "gl", "Galician" },
        { "gl-ES", "Galician - Galician" },
        { "gu", "Gujarati" },
        { "gu-IN", "Gujarati - India" },
        { "he", "Hebrew" },
        { "he-IL", "Hebrew - Israel" },
        { "hi", "Hindi" },
        { "hi-IN", "Hindi - India" },
        { "hr", "Croatian" },
        { "hr-HR", "Croatian - Croatia" },
        { "hu", "Hungarian" },
        { "hu-HU", "Hungarian - Hungary" },
        { "hy", "Armenian" },
        { "hy-AM", "Armenian - Armenia" },
        { "id", "Indonesian" },
        { "id-ID", "Indonesian - Indonesia" },
        { "is", "Icelandic" },
        { "is-IS", "Icelandic - Iceland" },
        { "it", "Italian" },
        { "it-CH", "Italian - Switzerland" },
        { "it-IT", "Italian - Italy" },
        { "ja", "Japanese" },
        { "ja-JP", "Japanese - Japan" },
        { "ka", "Georgian" },
        { "ka-GE", "Georgian - Georgia" },
        { "kk", "Kazakh" },
        { "kk-KZ", "Kazakh - Kazakhstan" },
        { "kn", "Kannada" },
        { "kn-IN", "Kannada - India" },
        { "ko", "Korean" },
        { "kok", "Konkani" },
        { "kok-IN", "Konkani - India" },
        { "ko-KR", "Korean - Korea" },
        { "ky", "Kyrgyz" },
        { "ky-KZ", "Kyrgyz - Kazakhstan" },
        { "lt", "Lithuanian" },
        { "Lt-az", "Azeri (Latin)" },
        { "Lt-az-AZ", "Azeri (Latin) - Azerbaijan" },
        { "lt-LT", "Lithuanian - Lithuania" },
        { "Lt-sr", "Serbian (Latin)" },
        { "Lt-sr-SP", "Serbian (Latin) - Serbia" },
        { "Lt-uz", "Uzbek (Latin)" },
        { "Lt-uz-UZ", "Uzbek (Latin) - Uzbekistan" },
        { "lv", "Latvian" },
        { "lv-LV", "Latvian - Latvia" },
        { "mk", "Macedonian (FYROM)" },
        { "mk-MK", "Macedonian (FYROM)" },
        { "mn", "Mongolian" },
        { "mn-MN", "Mongolian - Mongolia" },
        { "mr", "Marathi" },
        { "mr-IN", "Marathi - India" },
        { "ms", "Malay" },
        { "ms-BN", "Malay - Brunei" },
        { "ms-MY", "Malay - Malaysia" },
        { "nb", "Norwegian (Bokm├Ñl)" },
        { "nb-NO", "Norwegian (Bokm├Ñl) - Norway" },
        { "nl", "Dutch" },
        { "nl-BE", "Dutch - Belgium" },
        { "nl-NL", "Dutch - The Netherlands" },
        { "nn", "Norwegian (Nynorsk)" },
        { "nn-NO", "Norwegian (Nynorsk) - Norway" },
        { "pa", "Punjabi" },
        { "pa-IN", "Punjabi - India" },
        { "pl", "Polish" },
        { "pl-PL", "Polish - Poland" },
        { "pt", "Portuguese" },
        { "pt-BR", "Portuguese - Brazil" },
        { "pt-PT", "Portuguese - Portugal" },
        { "ro", "Romanian" },
        { "ro-RO", "Romanian - Romania" },
        { "ru", "Russian" },
        { "ru-RU", "Russian - Russia" },
        { "sa", "Sanskrit" },
        { "sa-IN", "Sanskrit - India" },
        { "sk", "Slovak" },
        { "sk-SK", "Slovak - Slovakia" },
        { "sl", "Slovenian" },
        { "sl-SI", "Slovenian - Slovenia" },
        { "sq", "Albanian" },
        { "sq-AL", "Albanian - Albania" },
        { "sv", "Swedish" },
        { "sv-FI", "Swedish - Finland" },
        { "sv-SE", "Swedish - Sweden" },
        { "sw", "Swahili" },
        { "sw-KE", "Swahili - Kenya" },
        { "syr", "Syriac" },
        { "syr-SY", "Syriac - Syria" },
        { "ta", "Tamil" },
        { "ta-IN", "Tamil - India" },
        { "te", "Telugu" },
        { "te-IN", "Telugu - India" },
        { "th", "Thai" },
        { "th-TH", "Thai - Thailand" },
        { "tr", "Turkish" },
        { "tr-TR", "Turkish - Turkey" },
        { "tt", "Tatar" },
        { "tt-RU", "Tatar - Russia" },
        { "uk", "Ukrainian" },
        { "uk-UA", "Ukrainian - Ukraine" },
        { "ur", "Urdu" },
        { "ur-PK", "Urdu - Pakistan" },
        { "vi", "Vietnamese" },
        { "vi-VN", "Vietnamese - Vietnam" },
        { "zh", "Chinese" },
        { "zh-CHS", "Chinese (Simplified)" },
        { "zh-CHT", "Chinese (Traditional)" },
        { "zh-CN", "Chinese - China" },
        { "zh-HK", "Chinese - Hong Kong SAR" },
        { "zh-MO", "Chinese - Macau SAR" },
        { "zh-SG", "Chinese - Singapore" },
        { "zh-TW", "Chinese - Taiwan" },
    };
}
