using Newtonsoft.Json;
using RTP.JsonReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRTP.JsonReaders
{
    public class JsonDataDeserealizer : IJsonDeserealizer
    {
        private Exception _lastException = null;
        public Exception GetLastException => _lastException;

        public T JsonDeserealizer<T>(string json_data)
        {
            if (string.IsNullOrEmpty(json_data))
                return default(T);
            try
            {
                _lastException = null;
                return JsonConvert.DeserializeObject<T>(json_data);
            }
            catch (Exception ex)
            {
                _lastException = ex;
                return default(T);
            }
        }
    }
}
