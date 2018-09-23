using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using wallstreeter.common.Model;
using wallstreeter.core.Bankier;
using wallstreeter.dal;
using wallstreeter.push;
using wallstreeter.push.Token;

namespace wallstreeter.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            SaveQuotations();
            //GeneratePushes();
        }

        public static void SaveQuotations()
        {
            var bankier = new Bankier();
            var stocks = new QuoteTableParser().Parse(bankier.GetStockTable().GetAwaiter().GetResult());

            using (var conn = new SqlConnection(@"Integrated Security=SSPI;Initial Catalog=Wallstreeter;Data Source=SKRZYNKA\SQLEXPRESS;"))
            {
                var repository = new QuotationsRepository(conn);
                repository.Insert(stocks);
            }
        }

        public static void GeneratePushes()
        {
            var bankier = new Bankier();
            var stocks = new QuoteTableParser().Parse(bankier.GetStockTable().GetAwaiter().GetResult());

            var messages = new List<Message>();
            var chunkSize = 20;
            for (int i = 0; i * chunkSize < stocks.Count; i++)
            {
                var stockChunk = stocks.Skip(i * chunkSize).Take(chunkSize).ToList();
                var tasks = new List<Task<List<Message>>>();
                foreach (var stock in stockChunk)
                {
                    Console.WriteLine(stock);
                    tasks.Add(new MessagesParser(stock.Name).Parse(bankier.GetQuoteInfo(stock.Name, DateTime.Today.AddDays(-2), DateTime.Today)));
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
