using System;
using System.Data;
using Dapper;
using wallstreeter.common.Model;
using System.Collections.Generic;

namespace wallstreeter.dal
{
    public class QuotationsRepository : IDisposable
    {
        private readonly IDbConnection _conn;

        public QuotationsRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public void Insert(List<Stock> stockInfo)
        {
            _conn.Execute(@"INSERT Quotations(Name, Quote, Change, ChangePercent, Transactions, Volume, [Open], Max, Min, TimeMod)
                            VALUES(@Name, @Quote, @Change, @ChangePercent, @Transactions, @Volume, @Open, @Max, @Min, @TimeMod)", stockInfo);   
        }

        public void Dispose()
        {
            _conn?.Dispose();
        }
    }
}
