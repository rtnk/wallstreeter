using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using wallstreeter.common.Model;

namespace wallstreeter.core
{
    public class Bankier
    {
        static HttpClient client = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            PreAuthenticate = true,
            UseDefaultCredentials = false,
            Credentials = CredentialCache.DefaultNetworkCredentials
        });

        private readonly string _baseBankierUrl = "https://www.bankier.pl";

        public readonly string Url = @"https://www.bankier.pl/inwestowanie/profile/quote.html?symbol={0}";
        public readonly string Messages = @"https://www.bankier.pl/gielda/notowania/akcje/{0}/komunikaty?start_dt={1}&end_dt={2}";

        public async Task<List<MessageInfo>> GetQuoteInfo(string shortName, DateTime dateStart, DateTime dateEnd)
        {
            var url = string.Format(Messages, shortName, dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"));
            var responseText = string.Empty;
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseText);
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='article']/header");
            var messages = new List<MessageInfo>();
            if (nodes == null)
            {
                return messages;
            }
            foreach (var node in nodes)
            {
                var aElement = node.Element("span").Element("a");
                var title = aElement.Attributes["title"].Value;
                var href = aElement.Attributes["href"].Value;
                var time = DateTime.Parse(node.Element("div").Element("time").InnerText);
                messages.Add(new MessageInfo
                {
                    Title = title,
                    Url = $"{_baseBankierUrl}{href}",
                    Time = time
                });
            }

            return messages;
        }
    }
}
