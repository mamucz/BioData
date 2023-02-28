using RTP.SQLWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP.BioToSQL
{
    public class HeartDataProvider
    {
        [TableName("SimSessionBioHeardRate")]
        public class Root
        {
            public string Time { get; set; }
            
            [ColumnName("SessionID")]
            public long SessionID { get; set; }
            [ColumnName("TIME")]
            public double TIME { get; set; }
            [ColumnName("Phase")]
            public int Phase { get; set; }
            [ColumnName("HR_UTS")]
            public Int64 UTS { get; set; }
            [ColumnName("HR")]
            public int HR_current { get; set; }
            public Int64 HR_current_time { get; set; }
            [ColumnName("HR_avg")]
            public int HR_avg { get; set; }
            [ColumnName("HR_max")]
            public int HR_max { get; set; }
            [ColumnName("HR_min")]
            public int HR_min { get; set; }
            [ColumnName("pNN30")]
            public int pNN30 { get; set; }
            [ColumnName("pNN50")]
            public int pNN50 { get; set; }
            [ColumnName("RMSSD")]
            public int RMSSD { get; set; }
            [ColumnName("SDNN")]
            public int SDNN { get; set; }
            [ColumnName("LF")]
            public int LF_power { get; set; }
            [ColumnName("HF")]
            public int HF_power { get; set; }
            [ColumnName("LFHF_RATIO")] 
            public int LF_to_HF_ratio { get; set; }
            [ColumnName("DFA1")]
            public int DFA1 { get; set; }
            [ColumnName("DFA2")]
            public int DFA2 { get; set; }
            [ColumnName("ApEn")]
            public int ApEn { get; set; }
            [ColumnName("VALIDITY")]
            public int VALIDITY { get; set; }
            public int WINDOW { get; set; }
            public string ID { get; set; }
            public string RECORDING { get; set; }
        }


    }
}
