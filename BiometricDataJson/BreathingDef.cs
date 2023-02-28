using Newtonsoft.Json;
using RTP.SQLWriter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP.BioToSQL
{ 
    public class BreathingDataProvider
    {
        public class DataframeToRows : IReadOnlyList<DataframeRow>, IEnumerable<DataframeRow>
        {
            Dataframe frame;
            long sessionID;
            int phase;
            double time;
            public DataframeToRows(Dataframe frame, long _SessionID, int _Phase, double _Time)
            {
                this.frame = frame;
                sessionID = _SessionID;
                phase = _Phase;
                time = _Time;
            }

            public DataframeRow this[int index] => new DataframeRow() { datamV = frame.datamV != null ? frame.datamV[index] : 0 , timems = frame.timems!=null ? frame.timems[index] : 0, cleaned = frame.cleaned != null ? frame.cleaned[index] : 0, SessionID = sessionID, Phase = phase, Time = time };
        
            public int Count => frame.datamV.Count;

            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return new DataframeRow() { datamV = frame.datamV != null ? frame.datamV[i] : 0 , timems = frame.timems!=null ? frame.timems[i] : 0, cleaned = frame.cleaned != null ? frame.cleaned[i] : 0, SessionID = sessionID, Phase = phase, Time = time };
                }
            }

            IEnumerator<DataframeRow> IEnumerable<DataframeRow>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return new DataframeRow() { datamV = frame.datamV != null ? frame.datamV[i] : 0, timems = frame.timems != null ? frame.timems[i] : 0, cleaned = frame.cleaned != null ? frame.cleaned[i] : 0, SessionID = sessionID, Phase = phase, Time = time };
                }
            }
        }
        public class DataframeRow
        {
            
            [ColumnName("SessionID")]
            public long SessionID { get; set; }
            [ColumnName("TIME")]
            public double Time { get; set; }
            [ColumnName("Phase")]
            public int Phase { get; set; }
            [ColumnName("datamV")]
            public double datamV { get; set; }
            [ColumnName("timems")]
            public double timems { get; set; }
            [ColumnName("cleaned")]
            public double cleaned { get; set; }
           
        }

       
        public class Dataframe//:IEnumerable<DataframeRows>
        {
            [JsonProperty("data[mV]")]
            public List<double> datamV { get; set; }

            [JsonProperty("time[ms]")]
            public List<double> timems { get; set; }

            [JsonProperty("cleaned[]")]
            public List<double> cleaned { get; set; }           
        }    
        

        public class PeakData
        {
            public Dataframe dataframe { get; set; }
        }

        public class RawData
        {
            public Dataframe dataframe { get; set; }
        }
        [TableName("SimSessionBioBrathRate")]
        public class RequiredFormat
        {
            [ColumnName("SessionID")]
            public long SessionID { get; set; }

            [ColumnName("TIME")]
            public double SystemTime { get; set; }

            [ColumnName("PHASE")]
            public int Phase { get; set; }

            [JsonProperty("RR_avg")]
            public string RR_avg { get; set; }

            [ColumnName("RR")]
            [JsonProperty("RR_current")]
            public string RR_current { get; set; }

            [JsonProperty("RR_current_time")]
            public string RR_current_time { get; set; }

            [ColumnName("RR_max")]
            [JsonProperty("RR_max")]
            public string RR_max { get; set; }


            [ColumnName("RR_min")]
            [JsonProperty("RR_min")]
            public string RR_min { get; set; }

            [JsonProperty("Time")]
            public string Time { get; set; }

            [ColumnName("RR_UTS")]
            [JsonProperty("UTS")]
            public string UTS { get; set; }

            [JsonProperty("WINDOW")]
            public string WINDOW { get; set; }

            public RequiredFormat()
            {
                RR_avg = "";
                RR_current = "";
                RR_current_time = "";
                RR_max = "";
                RR_min = "";
                Time = "";
                UTS = "";
                WINDOW = "";
            }
        }

        public class Root
        {
            public PeakData peakData { get; set; }
            public RawData rawData { get; set; }
            public RequiredFormat requiredFormat { get; set; }
        }
    }
}