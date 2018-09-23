using HtmlAgilityPack;
using System.Threading.Tasks;

namespace wallstreeter.core
{
    public interface ISiteParser<T>
    {
        T Parse(HtmlDocument doc);
        Task<T> Parse(Task<HtmlDocument> docAsync);
    }
}
