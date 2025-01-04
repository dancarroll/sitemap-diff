using System.Net;
using System.Xml.Linq;

namespace sitemap_diff
{
  class Program
  {
    static void Main(string[] args)
    {
      var ignoreDomain = Array.Exists(args, element => element == "--ignoreDomain");

      IEnumerable<string> originalUrls = PromptAndReadSitemap("Original: ", ignoreDomain);
      IEnumerable<string> newUrls = PromptAndReadSitemap("New: ", ignoreDomain);

      IEnumerable<string> intersection = originalUrls.Intersect(newUrls);

      IEnumerable<string> missingInNew = originalUrls.Where(s => !intersection.Contains(s));
      IEnumerable<string> newInNew = newUrls.Where(s => !intersection.Contains(s));

      Console.WriteLine("\nMissing in new sitemap");
      Console.WriteLine("----------------------");
      foreach (string s in missingInNew)
      {
        Console.WriteLine(s);
      }

      Console.WriteLine("\nNew in new sitemap");
      Console.WriteLine("------------------");
      foreach (string s in newInNew)
      {
        Console.WriteLine(s);
      }

      Console.WriteLine();
    }

    static IEnumerable<string> PromptAndReadSitemap(string prompt, bool ignoreDomain)
    {
      Console.Write(prompt);
      string url = Console.ReadLine();

      WebClient client = new WebClient();
      string sitemapXml = client.DownloadString(url);

      XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
      XDocument xDoc = XDocument.Load(url);
      var x = from el in xDoc.Element(ns + "urlset").Elements()
              select (ignoreDomain ?
                new Uri(el.Element(ns + "loc").Value).PathAndQuery
                : el.Element(ns + "loc").Value);

      return x;
    }
  }
}
