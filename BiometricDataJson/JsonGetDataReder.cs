using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RTP.JsonReaders
{
    public class JsonGetDataReder : IJsonWebReader
    {
        Exception _lastException;
        public Exception GetLastException => _lastException;
        
        public string ReadJson(string url)
        {
            string json_data = string.Empty;
            try
            {
                Uri uri = new Uri(url);

                string ipAddress = uri.Host;
                int port = uri.Port;
                string app = uri.AbsolutePath.Substring(1);

                // Vytvorim TCP/IP Socket
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(ipAddress, port);
                    // Pripojim Socket
                    // Log ze to probehlo
                    // odelsu na server GET request
                    string request = "GET /hrvparams HTTP/1.1\r\nHost: " + ipAddress + ":" + port + "\r\n\r\n";
                    byte[] messageSent = Encoding.ASCII.GetBytes(request+ "\r\n");
                    int byteSent = socket.Send(messageSent);

                    // prijmu ze serveru co vratil
                    byte[] buffer = new byte[1024];
                    int bytesRead = socket.Receive(buffer);
                    
                    json_data = System.Text.Encoding.Default.GetString(buffer);
                    int pos = json_data.IndexOf("{");
                    json_data = json_data.Substring(pos);
                    // zavru Socket
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }

                // Osetreni chyb socketove komunikace
                catch (ArgumentNullException ane)
                {
                    _lastException = ane;
                }

                catch (SocketException se)
                {

                    _lastException = se;
                }

                catch (Exception e)
                {
                    _lastException = e;
                }
            }

            // projistotu obalene cele, kdyby nastala vyjimka
            catch (Exception e)
            {

                _lastException = e;
            }
            return json_data;
        }
    }
}
