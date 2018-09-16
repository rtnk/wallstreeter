using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using wallstreeter.common.Model;
using wallstreeter.core;
using wallstreeter.push;
using wallstreeter.push.Token;

namespace wallstreeter.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var bankier = new Bankier();
            var stocks = bankier.GetStockNames().GetAwaiter().GetResult();
                        
            var messages = new List<MessageInfo>();
            var chunkSize = 20;
            for(int i = 0; i * chunkSize < stocks.Count; i++)
            {
                var stockChunk = stocks.Skip(i * chunkSize).Take(chunkSize).ToList();
                var tasks = new List<Task<List<MessageInfo>>>();
                foreach (var stock in stockChunk)
                {
                    Console.WriteLine(stock);
                    tasks.Add(bankier.GetQuoteInfo(stock.Name, DateTime.Today.AddDays(-2), DateTime.Today));
                }
                var results = Task.WhenAll(tasks).GetAwaiter().GetResult();
                messages.AddRange(results.SelectMany(x => x));
            }

            var tokenFilePath = ConfigurationManager.AppSettings["TokenFilePath"];
            var push = new PushBullet(new FileToken(tokenFilePath));

            foreach (var message in messages)
            {
                Console.WriteLine($"##### {message.StockName} - {message.Time} #####");
                Console.WriteLine($"{message.Title}");
                Console.WriteLine($"{message.Url}");

                push.Push($"{message.StockName} - {message.Time.ToString("yyyy-MM-dd HH:mm:ss")}", message.Title, message.Url);
            }
        }
    }
}
