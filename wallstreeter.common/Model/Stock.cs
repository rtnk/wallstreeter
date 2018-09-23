using System;

namespace wallstreeter.common.Model
{
    public class Stock
    {
        public string Name { get; set; }
        public decimal Quote { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public int Transactions { get; set; }
        public int Volume { get; set; }
        public decimal Open { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public DateTime? TimeMod { get; set; }
    }
}
