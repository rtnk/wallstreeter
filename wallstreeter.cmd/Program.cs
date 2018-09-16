using System;
using System.Configuration;
using wallstreeter.core;
using wallstreeter.push;
using wallstreeter.push.Token;

namespace wallstreeter.cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockShortName = "MBANK";
            var bankier = new Bankier();
            var messages = bankier.GetQuoteInfo(stockShortName, DateTime.Today.AddDays(-5), DateTime.Today).GetAwaiter().GetResult();

            Console.WriteLine($"##### {stockShortName} #####");

            var tokenFilePath = ConfigurationManager.AppSettings["TokenFilePath"];
            var push = new PushBullet(new FileToken(tokenFilePath));
            
            foreach (var message in messages)
            {
                Console.WriteLine($"##### {message.Time} #####");
                Console.WriteLine($"{message.Title}");
                Console.WriteLine($"{message.Url}");

                push.Push($"{stockShortName} - {message.Time.ToString("yyyy-MM-dd HH:mm:ss")}", message.Title, message.Url);
            }
        }
    }
}
