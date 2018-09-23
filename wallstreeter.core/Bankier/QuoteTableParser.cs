using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using wallstreeter.common.Model;
using System.Globalization;
using System.Threading.Tasks;

namespace wallstreeter.core.Bankier
{
    public class QuoteTableParser : ISiteParser<List<Stock>>
    {
        public async Task<List<Stock>> Parse(Task<HtmlDocument> docAsync)
        {
            var doc = await docAsync;
            return Parse(doc);
        }

        public List<Stock> Parse(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//section[@id='quotes']//tbody/tr");
            var stockNames = new List<Stock>();
            if (nodes == null)
            {
                return stockNames;
            }
            foreach (var node in nodes)
            {
                var stock = new Stock();
                try
                {
                    stock.Name = GetNodeChildText(node, "colWalor");
                    stock.Quote = Convert.ToDecimal(GetNodeChildText(node, "colKurs").Replace("&nbsp;", ""), new NumberFormatInfo { NumberDecimalSeparator = "," });
                    stock.Change = Convert.ToDecimal(GetNodeChildText(node, "colZmiana").Replace("&nbsp;", ""), new NumberFormatInfo { NumberDecimalSeparator = "," });
                    stock.ChangePercent = Convert.ToDecimal(GetNodeChildText(node, "colZmianaProcentowa").Replace("&nbsp;", "").Replace("%", ""), new NumberFormatInfo { NumberDecimalSeparator = "," });
                    stock.Transactions = Convert.ToInt32(GetNodeChildText(node, "colLiczbaTransakcji").Replace("&nbsp;", ""));
                    stock.Volume = Convert.ToInt32(GetNodeChildText(node, "colObrot").Replace("&nbsp;", ""));
                    stock.Open = Convert.ToDecimal(GetNodeChildText(node, "colOtwarcie").Replace("&nbsp;", ""));
                    stock.Max = Convert.ToDecimal(GetNodeChildText(node, "calMaxi").Replace("&nbsp;", ""), new NumberFormatInfo { NumberDecimalSeparator = "," });
                    stock.Min = Convert.ToDecimal(GetNodeChildText(node, "calMini").Replace("&nbsp;", ""), new NumberFormatInfo { NumberDecimalSeparator = "," });
                    stock.TimeMod = ConvertToDateTime(GetNodeChildText(node, "colAktualizacja"));

                    stockNames.Add(stock);
                }
                catch (Exception ex)
                {
                    //log something here...
                }
            }

            return stockNames;
        }

        private DateTime? ConvertToDateTime(string date)
        {
            if (date.Length != 11)
            {
                return null;
            }
            var day = Convert.ToInt32(date.Substring(0, 2));
            var month = Convert.ToInt32(date.Substring(3, 2));
            var hour = Convert.ToInt32(date.Substring(6, 2));
            var minute = Convert.ToInt32(date.Substring(9, 2));
            var year = month > DateTime.Now.Month || (month == DateTime.Now.Month && day > DateTime.Now.Day) ? DateTime.Now.Year - 1 : DateTime.Now.Year;

            return new DateTime(year, month, day, hour, minute, 0);
        }

        private string GetNodeChildText(HtmlNode node, string className)
        {
            return node.ChildNodes.Where(x => x.HasClass(className)).FirstOrDefault()?.InnerText.Trim();
        }
    }
}
