using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace wallstreeter.core.Bankier
{
    public class Bankier
    {
        public readonly static string BaseUrl = "https://www.bankier.pl";

        public readonly string StockUrl = $@"/gielda/notowania/akcje";
        public readonly string StockNcUrl = $@"/gielda/notowania/new-connect";

        public readonly string Quote = @"/inwestowanie/profile/quote.html?symbol={0}";
        public readonly string Messages = @"/gielda/notowania/akcje/{0}/komunikaty?start_dt={1}&end_dt={2}";

        private readonly Site _bankierSite;

        public Bankier()
        {
            _bankierSite = new Site(BaseUrl);
        }

        public async Task<HtmlDocument> GetStockTable()
        {
            return await _bankierSite.GetHtmlDocument(StockUrl);
        }

        public async Task<HtmlDocument> GetStockNcTable()
        {
            return await _bankierSite.GetHtmlDocument(StockNcUrl);
        }

        public async Task<HtmlDocument> GetQuoteInfo(string shortName, DateTime dateStart, DateTime dateEnd)
        {
            var url = string.Format(Messages, shortName, dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"));
            return await _bankierSite.GetHtmlDocument(url);
        }
    }
}
