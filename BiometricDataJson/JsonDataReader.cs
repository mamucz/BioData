using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public interface IJsonDataReader
    {
        Task ReadDataAsync();
    }
    public class JsonDataReader<T>
    {
        public IJsonWebReader JsonWebReader { get => _webReader; set => _webReader = value; }
        public IJsonDeserealizer JsonDeserealizer { get => _deserializer; set => _deserializer = value; }
        IJsonWebReader _webReader;
        IJsonDeserealizer _deserializer;

        public JsonDataReader(IJsonWebReader jsonWebReader, IJsonDeserealizer jsonDeserealizer)
        {
            _webReader = jsonWebReader;
            _deserializer = jsonDeserealizer;
        }

        public JsonDataReader()
        {
        }
        public Task<T> ReadDataAsync(string url)
        {
            return Task.Run(() => ReadData(url));
        }

        public T ReadData(string url)
        {
            var sjson = ReadJsonStringAsync(url);
            return DeserealizeJsonAsync(sjson);
        }

        private string ReadJsonStringAsync(string url)
        {
             return _webReader.ReadJson(url);            
        }
        private T DeserealizeJsonAsync(string json)
        {           
            return _deserializer.JsonDeserealizer<T>(json);
        }
    }
}
