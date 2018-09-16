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

        private readonly static string _baseBankierUrl = "https://www.bankier.pl";

        public readonly string StockUrl = $@"{_baseBankierUrl}/gielda/notowania/akcje";

        public readonly string Url = _baseBankierUrl + @"/inwestowanie/profile/quote.html?symbol={0}";
        public readonly string Messages = _baseBankierUrl + @"/gielda/notowania/akcje/{0}/komunikaty?start_dt={1}&end_dt={2}";

        public async Task<List<StockInfo>> GetStockNames()
        {
            var responseText = string.Empty;
            HttpResponseMessage response = await client.GetAsync(StockUrl);
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseText);
            var nodes = doc.DocumentNode.SelectNodes("//section[@id='quotes']//tbody/tr");
            var stockNames = new List<StockInfo>();
            if (nodes == null)
            {
                return stockNames;
            }
            foreach(var node in nodes)
            {
                var stock = new StockInfo
                {
                    Name = node.ChildNodes.Where(x => x.HasClass("colWalor")).FirstOrDefault()?.InnerText.Trim(),
                    Quote = node.ChildNodes.Where(x => x.HasClass("colKurs")).FirstOrDefault()?.InnerText.Trim(),
                    Change = node.ChildNodes.Where(x => x.HasClass("colZmiana")).FirstOrDefault()?.InnerText.Trim(),
                    PercentChange = node.ChildNodes.Where(x => x.HasClass("colZmianaProcentowa")).FirstOrDefault()?.InnerText.Trim(),
                    TransactionsCount = node.ChildNodes.Where(x => x.HasClass("colObrot")).FirstOrDefault()?.InnerText.Trim(),
                    Volume = node.ChildNodes.Where(x => x.HasClass("colObrot")).FirstOrDefault()?.InnerText.Trim(),
                    Open = node.ChildNodes.Where(x => x.HasClass("colOtwarcie")).FirstOrDefault()?.InnerText.Trim(),
                    Max = node.ChildNodes.Where(x => x.HasClass("calMaxi")).FirstOrDefault()?.InnerText.Trim(),
                    Min = node.ChildNodes.Where(x => x.HasClass("calMini")).FirstOrDefault()?.InnerText.Trim(),
                    Time = node.ChildNodes.Where(x => x.HasClass("colAktualizacja")).FirstOrDefault()?.InnerText.Trim()
                };
                
                stockNames.Add(stock);
            }

            return stockNames;
        }

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
                    StockName = shortName,
                    Title = title,
                    Url = $"{_baseBankierUrl}{href}",
                    Time = time
                });
            }

            return messages;
        }
    }
}
