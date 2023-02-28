using logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RTP.SQLWriter;

using RTP.JsonReaders;
using RRTP.JsonReaders;

namespace RTP.BioToSQL
{
    public class BioToSQL : Paradigma
    {
        public ParadigmaIO SessionID = null;
        public ParadigmaIO TIME = null;
        public ParadigmaIO Phase = null;
        public ParadigmaIO Enable = null;
        public ParadigmaIO Error = null;
      
        public bool IsRunning = false;
        private bool first = true;

        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string SqlServerName { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string SqlInstanceName { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string UserName { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string Password { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string Database { get; set; }

        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string breathServerURL  { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string heartServerURL { get; set; }
        [LabPropertyAttribute(LabPropertyAttribute.AttType.VISUAL)]
        public string emotionsServerURL { get; set; }


        JsonReader bioreader = new JsonReader();

        JsonWebDataReder jsonWebReader = new JsonWebDataReder();
        JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();

        JsonReader.JsonRederItem<BreathingDataProvider.Root> breathRederItem;
        JsonReader.JsonRederItem<HeartDataProvider.Root> heartRederItem;
        JsonReader.JsonRederItem<EmotionDataProvider.Root> emotionRederItem;

        SqlRepository<EmotionDataProvider.EmotionsSql> sqlEmotions;
        SqlRepository<HeartDataProvider.Root> sqlHeart;
        SqlRepository<BreathingDataProvider.RequiredFormat> sqlBreath;
        SqlRepository<BreathingDataProvider.DataframeRow> sqlBreathPeeks;
        SqlRepository<BreathingDataProvider.DataframeRow> sqlBreathRaw;

        double lastRawtimems = 0;
        double lastPeektimems = 0;
        
        private string ConnectionString {
            get {
                return String.Format("Data Source = {0}\\{1}; Initial Catalog = {2}; User ID = {3}; Password = {4}; MultipleActiveResultSets = True", SqlServerName, SqlInstanceName, Database, UserName, Password);
            }
        }

        public BioToSQL()
        {
            Type = "BioToSQL";
            Sign = "BTOS";
        }
       
        public override void AddOutput()
        {
            base.AddOutput();
        }

        public override void Init(Theorem th)
        {
            breathRederItem = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            heartRederItem = bioreader.AddReader<HeartDataProvider.Root>(heartServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            emotionRederItem = bioreader.AddReader<EmotionDataProvider.Root>(emotionsServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);

            breathRederItem.DataReady += BreathRederItem_DataReady;
            heartRederItem.DataReady += HeartRederItem_DataReady;
            emotionRederItem.DataReady += EmotionRederItem_DataReady;

            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider(SqlServerName, SqlInstanceName, UserName, Password,Database);
            sqlEmotions = new SqlRepository<EmotionDataProvider.EmotionsSql>(sqlp);
            sqlHeart = new SqlRepository<HeartDataProvider.Root>(sqlp);
            sqlBreath = new SqlRepository<BreathingDataProvider.RequiredFormat>(sqlp);
            sqlBreathPeeks = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathPeekData");
            sqlBreathRaw = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathRawData");

            base.Init(th);
        }

        private void EmotionRederItem_DataReady(object sender, EmotionDataProvider.Root args)
        {
            WriteToSQLAsync(args);
        }
        private void HeartRederItem_DataReady(object sender, HeartDataProvider.Root args)
        {
            WriteToSQLAsync(args);
        }
        private void BreathRederItem_DataReady(object sender, BreathingDataProvider.Root args)
        {
            WriteToSQLAsync(args);
        }

        private void WriteToSql(BreathingDataProvider.Root data)
        {
            sqlBreath.Insert(data.requiredFormat);

            //Tranform columns to raw
            var peeks = new BreathingDataProvider.DataframeToRows(data.peakData.dataframe, SessionID.GetAsLong(), Phase.GetAsInt(), TIME.GetAsDouble());
            var raws = new BreathingDataProvider.DataframeToRows(data.rawData.dataframe, SessionID.GetAsLong(), Phase.GetAsInt(), TIME.GetAsDouble());

            // Find data that has been added since last read
            var newraws = raws.Where(x => x.timems > lastRawtimems).ToArray();
            var newpeeks = peeks.Where(x => x.timems > lastPeektimems).ToArray();
            // Save the last most recent time
            if (newpeeks.Length != 0)
                lastPeektimems = newpeeks.Max(x => x.timems);
            if (newraws.Length != 0)
                lastRawtimems = newraws.Max(x => x.timems);
            // Do not store on first pass
            if (first == false)
            {
                for (int i = 0; i < newpeeks.Count(); i++)
                {
                    newpeeks[i].SessionID = SessionID.GetAsLong();
                    newpeeks[i].Phase = Phase.GetAsInt();
                    newpeeks[i].Time = TIME.GetAsDouble();
                    sqlBreathPeeks.Insert(newpeeks[i]);
                }
            }

            if (first == false)
            {
                for (int i = 0; i < newraws.Count(); i++)
                {
                    newraws[i].SessionID = SessionID.GetAsLong();
                    newraws[i].Phase = Phase.GetAsInt();
                    newraws[i].Time = TIME.GetAsDouble();
                    sqlBreathRaw.Insert(newraws[i]);
                }
            }
            first = false;
        }
        private Task WriteToSQLAsync(BreathingDataProvider.Root data)
        {
            return new Task(() => {
                WriteToSql(data);
            });
        }


        private void WriteToSql(HeartDataProvider.Root data)
        {
            sqlHeart.Insert(data);
        }
        private Task WriteToSQLAsync(HeartDataProvider.Root data)
        {
            return new Task(() => {
                WriteToSql(data);
            });
        }

        private void WriteToSql(EmotionDataProvider.Root data)
        {
            sqlEmotions.Insert(new EmotionDataProvider.EmotionsSql(data));
        }
        private Task WriteToSQLAsync(EmotionDataProvider.Root data)
        {
            return new Task(() => {
                WriteToSql(data);
            });
        }
        public override void Prepare(Theorem th)
        {
            //base.Prepare(th);
            Inputs.Add(new ParadigmaIO("SessionID", ParadigmaIO.TYPE.Input, this));
            Inputs.Add(new ParadigmaIO("TIME", ParadigmaIO.TYPE.Input, this));
            Inputs.Add(new ParadigmaIO("Phase", ParadigmaIO.TYPE.Input, this));
            Inputs.Add(new ParadigmaIO("Enable", ParadigmaIO.TYPE.Input, this));
            Outputs.Add(new ParadigmaIO("Error", ParadigmaIO.TYPE.Output, this));
            SetIOReferencis();
        }

        public override void SetIOReferencis()
        {
            SessionID = GetIOByName("SessionID", ParadigmaIO.TYPE.Input);
            TIME = GetIOByName("TIME", ParadigmaIO.TYPE.Input);
            Phase = GetIOByName("Phase", ParadigmaIO.TYPE.Input);
            Enable = GetIOByName("Enable", ParadigmaIO.TYPE.Input);
            Error = GetIOByName("Error", ParadigmaIO.TYPE.Output);
        }

        public override void Process()
        {
            if (IsRunning == true && Enable.GetAsBool() == false)
            { 
                bioreader.StopContinualReading();
                IsRunning = false;
            }
            if (IsRunning == false && Enable.GetAsBool() == true)
            {
                IsRunning = true;
                bioreader.StartContinualReading();
            }
        }

        public override void ProcessStart(Theorem theorem)
        {
            IsRunning = true;
            base.ProcessStart(theorem);
            bioreader.StartContinualReading();
        }

        public override void ProcessStop(Theorem theorem)
        {
            base.ProcessStop(theorem);
            bioreader.StopContinualReading();
            IsRunning = false;
        }

        public override void FromCustomXElements(XElement xelements)
        {
            SqlServerName = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "SqlServerName").Attribute("Value").Value.ToString();
            SqlInstanceName = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "SqlInstanceName").Attribute("Value").Value.ToString();
            UserName = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "UserName").Attribute("Value").Value.ToString();
            Password = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "Password").Attribute("Value").Value.ToString();
            Database = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "Database").Attribute("Value").Value.ToString();
            breathServerURL = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "breathServerURL").Attribute("Value").Value.ToString();
            heartServerURL = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "heartServerURL").Attribute("Value").Value.ToString();
            emotionsServerURL = xelements.Elements("Property").FirstOrDefault(x => x.Attribute("Name").Value == "emotionsServerURL").Attribute("Value").Value.ToString();
        }

        public override XElement[] GetCustomXElements()
        {
            return new XElement[] { new XElement("Property", new XAttribute("Name","SqlServerName"), new XAttribute("Value", SqlServerName), new XAttribute("Type", SqlServerName.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","SqlInstanceName"), new XAttribute("Value", SqlInstanceName), new XAttribute("Type", SqlInstanceName.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","UserName"), new XAttribute("Value", UserName), new XAttribute("Type", UserName.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","Password"), new XAttribute("Value", Password), new XAttribute("Type", Password.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","Database"), new XAttribute("Value", Database), new XAttribute("Type", Database.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","breathServerURL"), new XAttribute("Value", breathServerURL), new XAttribute("Type", breathServerURL.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","heartServerURL"), new XAttribute("Value", heartServerURL), new XAttribute("Type", heartServerURL.GetType().FullName)),
                                    new XElement("Property", new XAttribute("Name","emotionsServerURL"), new XAttribute("Value", emotionsServerURL), new XAttribute("Type", emotionsServerURL.GetType().FullName)),};

        }

        public override void Dispose()
        {
            base.Dispose();
            if (bioreader!=null)
                bioreader.Dispose();
            sqlEmotions.Dispose();
            sqlHeart.Dispose();
            sqlBreath.Dispose();
            sqlBreathPeeks.Dispose();
            sqlBreathRaw.Dispose();
        }
    }

    
}
