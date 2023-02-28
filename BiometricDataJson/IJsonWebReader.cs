using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public interface IJsonWebReader
    {
        string ReadJson(string url);
        Exception GetLastException { get; }
    }
}
