using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public class JsonWebDataReder : IJsonWebReader
    {
        private Exception _lastException = null;
        public Exception GetLastException => throw new NotImplementedException();
        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 3000;
                return w;
            }
        }

        private MyWebClient webc = new MyWebClient();

      

        public string ReadJson(string url)
        {    
            string s = null;
            try
            {
                _lastException = null;
                s = webc.DownloadString(url);
            }
            catch (Exception ex)
            {
               _lastException = ex; 
               return null;
            }
            return s;
            
        }
    }
}
