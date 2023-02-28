using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public interface IJsonDeserealizer
    {
        Exception GetLastException { get; }
        T JsonDeserealizer<T>(string json_data);        
    }
}
