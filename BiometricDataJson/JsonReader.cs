using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public delegate void DataReadyDelegate<T>(object sender,T data);

    public interface IRequestCounter
    {
        void IncrementRequestCount();
        void DecrementRequestCount();
    }
    public class JsonReader: IRequestCounter, IDisposable
    {
        
        private int _runningRequests = 0;
        public int RunningRequests { get => _runningRequests; }
        public interface IJsonReaderItem
        {          
            void ReadDataAsync(int repeats = 1);           
            void StartContinualReading();
            void StopContinualReading();
        }

        public class JsonRederItem<T> : IJsonReaderItem,IDisposable
        {
            public delegate void DataReadyEventHandler(object sender, T args);
            public event DataReadyEventHandler DataReady;

            public long CallCount { get => _callCount; }
            private long _callCount = 0;
            private JsonDataReader<T> _jsonDataReader = new JsonDataReader<T>();
            public JsonDataReader<T> jsonDataReader { get => _jsonDataReader; }
            public T Data { get; set; }
            public string url;
            private IRequestCounter _requestCounter;
            private int count = 0;
            private int maxCount = 1;
            private bool reading = false;
            public Type Type {
                get {
                    return typeof(T);
                }
            }

            
            private void OnDataReady(object sender, T args)
            {
                if (DataReady != null)
                    DataReady(sender, args);
            }
            public void ReadDataAsync(int repeats = 1)
            {
                maxCount = repeats;
                count = 0;
                Read();
            }
            public T ReadData()
            {
                return _jsonDataReader.ReadData(url);
            }
            public void StartContinualReading()
            {
                reading = true;
                count = 1;
                maxCount = 0;
                Read();
            }

            public void StopContinualReading()
            {
                reading = false;
            }

            private async void Read()
            {
                _requestCounter.IncrementRequestCount();
                Data = await _jsonDataReader.ReadDataAsync(url);
                _requestCounter.DecrementRequestCount();
                _callCount++;
                if (++count < maxCount || reading)
                    Read();
                return;
            
            }

            public void Dispose()
            {
                reading = false;
                count = maxCount + 1;
            }

            public JsonRederItem(IRequestCounter requestCounter, IJsonWebReader jsonWebReader, IJsonDeserealizer jsonDeserealizer)            
            {
                _requestCounter = requestCounter;
                jsonDataReader.JsonWebReader = jsonWebReader;
                jsonDataReader.JsonDeserealizer = jsonDeserealizer;
            }
           
        }
        List<IJsonReaderItem> list = new List<IJsonReaderItem>();
       
   
       

        public JsonRederItem<T> AddReader<T>(string url, IRequestCounter requestCounter, IJsonWebReader jsonWebReader, IJsonDeserealizer jsonDeserealizer)
        {
            var node = new JsonRederItem<T>(requestCounter,jsonWebReader, jsonDeserealizer) { url = url };
            list.Add(node);
            return node;
        }

        public void ReadData()
        {
            foreach (var n in list)
            {
                n.ReadDataAsync();
            }
        }
     
        public void ReadDataAsync()
        {            
            foreach(var n in list)
            {
                n.ReadDataAsync();
            }
        }
        public void ReadDataAsync(int repeats = 1)
        {            
            foreach (var n in list)
            {
                n.ReadDataAsync(repeats);
            }
        }
        public void StartContinualReading()
        {
            foreach (var n in list)
            {
                n.StartContinualReading();
            }            
        }

        public void StopContinualReading()
        {
            foreach (var n in list)
            {
                n.StopContinualReading();
            }
        }
        public void IncrementRequestCount()
        {
            Interlocked.Increment(ref _runningRequests);
        }

        public void DecrementRequestCount()
        {
            Interlocked.Decrement(ref _runningRequests);
        }

        public void Dispose()
        {
            foreach(var n in list)
                (n as IDisposable).Dispose();
            list.Clear();
        }
    }
}
