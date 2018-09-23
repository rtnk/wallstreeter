using HtmlAgilityPack;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace wallstreeter.core
{
    public class Site
    {
        static HttpClient client = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            PreAuthenticate = true,
            UseDefaultCredentials = false,
            Credentials = CredentialCache.DefaultNetworkCredentials
        });

        public Site(string baseAddress)
        {
            client.BaseAddress = new Uri(baseAddress);
        }

        public async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            var responseText = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseText);
            return doc;
        }
    }
}
