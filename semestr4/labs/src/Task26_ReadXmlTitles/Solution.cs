using System.Xml.Linq;

namespace Task26_ReadXmlTitles;

public static class XmlTitleReader
{
    public static List<string> ReadTitles(string xmlPath)
        => XDocument.Load(xmlPath)
                    .Descendants("title")
                    .Select(x => x.Value)
                    .ToList();
}
