using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using wallstreeter.common.Model;
using System.Threading.Tasks;

namespace wallstreeter.core.Bankier
{
    public class MessagesParser : ISiteParser<List<Message>>
    {
        private readonly string _shortName;

        public MessagesParser(string shortName)
        {
            _shortName = shortName;
        }

        public async Task<List<Message>> Parse(Task<HtmlDocument> docAsync)
        {
            var doc = await docAsync;
            return Parse(doc);
        }

        public List<Message> Parse(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='article']/header");
            var messages = new List<Message>();
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
                messages.Add(new Message
                {
                    StockName = _shortName,
                    Title = title,
                    Url = $"{Bankier.BaseUrl}{href}",
                    Time = time
                });
            }

            return messages;
        }
    }
}
