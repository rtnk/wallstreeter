using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wallstreeter.core;

namespace wallstreeter.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockShortName = "MBANK";
            var bankier = new Bankier();
            var messages = bankier.GetQuoteInfo(stockShortName, DateTime.Today.AddMonths(-1), DateTime.Today).GetAwaiter().GetResult();

            Console.WriteLine($"##### {stockShortName} #####");
            foreach (var message in messages)
            {
                Console.WriteLine($"##### {message.Time} #####");
                Console.WriteLine($"{message.Title}");
                Console.WriteLine($"{message.Url}");
            }
            Console.ReadKey();
        }
    }
}
