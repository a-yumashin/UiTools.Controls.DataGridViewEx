using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal class Strings
    {
        internal static Strings Instance { get; private set; }

        internal string CurrentLocale { get; set; } = "en";
        internal List<string> SupportedLocales => supportedLocales;

        private readonly StringTable stringTable;
        private readonly List<string> supportedLocales;

        static Strings()
        {
            Instance = new Strings();
        }

        internal Strings()
        {
            stringTable = CommonStuff.Deserialize<StringTable>(
                CommonStuff.GetEmbeddedResource("Resources.Strings.xml"));
            supportedLocales = stringTable.StringTableRecords
                .SelectMany(rec => rec.StringValues.Select(v => v.Locale))
                .Distinct()
                .OrderBy(v => v)
                .ToList();
        }

        internal string SR(string stringInEnglish)
        {
            return string.IsNullOrEmpty(CurrentLocale) || CurrentLocale == "en" || !SupportedLocales.Contains(CurrentLocale)
                ? stringInEnglish
                : GetLocalizedString(stringInEnglish);
        }

        private string GetLocalizedString(string stringInEnglish)
        {
            var entry = stringTable.StringTableRecords
                .FirstOrDefault(p => p.StringValues.Any(v => v.Locale == "en" && v.Value == stringInEnglish));
            if (entry == null)
                return stringInEnglish;
            var localizedStringValue = entry.StringValues.FirstOrDefault(v => v.Locale == CurrentLocale);
            return localizedStringValue == null
                ? stringInEnglish
                : localizedStringValue.Value;
        }
    }

    public class StringTable
    {
        [XmlElement("String")]
        public StringTableRecord[] StringTableRecords { get; set; }
    }
    public class StringTableRecord
    {
        [XmlElement("Value")]
        public StringValue[] StringValues { get; set; }
    }
    public class StringValue
    {
        [XmlAttribute("locale")]
        public string Locale { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
