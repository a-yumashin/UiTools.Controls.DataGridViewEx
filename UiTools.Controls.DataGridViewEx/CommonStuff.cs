using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class CommonStuff
    {
        internal static string GetEmbeddedResource(string res)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("UiTools.Controls.ExtendedDataGridView." + res)))
            {
                return reader.ReadToEnd();
            }
        }

        internal static T Deserialize<T>(string objXml) where T : class
        {
            if (string.IsNullOrEmpty(objXml))
                return null;
            try
            {
                T obj = null;
                var xs = new XmlSerializer(typeof(T));
                using (var sr = new StringReader(objXml))
                {
                    var xtr = new XmlTextReader(sr);
                    obj = xs.Deserialize(xtr) as T;
                    xtr.Close();
                    sr.Close();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialization failed: " + ex.Message, ex);
            }
        }

        internal static string SR(string stringInEnglish) // just a shorthand
        {
            return Strings.Instance.SR(stringInEnglish);
        }
    }
}
