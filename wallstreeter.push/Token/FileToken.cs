using System.Configuration;
using System.IO;

namespace wallstreeter.push.Token
{
    public class FileToken : IPushToken
    {
        public string Path { get; private set; }

        public FileToken(string path)
        {
            Path = path;
        }
        public string Get()
        {
            return File.ReadAllText(Path);
        }
    }
}
