using System.Text;
using System.Xml.Linq;

namespace Devdiscourse.Utility
{
    public class SitemapService
    {
        public string GenerateSitemap(List<SitemapUrl> urls)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"),
                    from url in urls
                    select new XElement(ns + "url",
                        new XElement(ns + "loc", url.Loc),
                        url.LastMod != null ? new XElement(ns + "lastmod", url.LastMod) : null,
                        url.ChangeFreq != null ? new XElement(ns + "changefreq", url.ChangeFreq) : null,
                        new XElement(ns + "priority", url.Priority)
                    )
                )
            );

            using var sw = new Utf8StringWriter();
            sitemap.Save(sw);
            return sw.ToString();
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public class SitemapUrl
    {
        public required string Loc { get; set; }
        public string? LastMod { get; set; }
        public string? ChangeFreq { get; set; }
        public required string Priority { get; set; }
    }

}
