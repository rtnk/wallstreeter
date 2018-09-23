using System;

namespace wallstreeter.common.Model
{
    public class Message
    {
        public string StockName { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
