using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using RTP.SQLWriter;
using RTP.BioToSQL;
using RRTP.JsonReaders;
using RTP.JsonReaders;

namespace BiometricDataJsonTest
{
    [TestClass]
    public class JsonReaderTest
    {
        private static string breathServerURL = @"http://192.168.168.4:8082/hrvparams";
        private static string heartServerURL = @"http://192.168.168.4:8081/hrvparams";
        private static string emotionsServerURL = @"http://192.168.168.4:5000/face-tracker";

        [TestMethod]
        public async Task OneCall()
        {
            Stopwatch sw = new Stopwatch();

            JsonReader bioreader = new JsonReader();

            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();        
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            breathingNode.DataReady += Breathing_DataReady;
            sw.Start();
            bioreader.ReadDataAsync();          
            sw.Stop();
            // Test if calling is realy assinchronus
            Assert.AreEqual(null, breathingNode.Data);
            await Task.Delay(5000);
            Assert.AreNotEqual(null, breathingNode.Data.requiredFormat);
            Assert.AreEqual(breathingNode.CallCount, 1);
        }
        [TestMethod]
        public void TestRunnigCounter()
        {
            Stopwatch sw = new Stopwatch();

            JsonReader bioreader = new JsonReader();

            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode1 = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            var breathingNode2 = bioreader.AddReader<HeartDataProvider.Root>(heartServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            var breathingNode3 = bioreader.AddReader<EmotionDataProvider.Root>(emotionsServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            sw.Start();
            bioreader.ReadDataAsync();
            Assert.IsTrue(bioreader.RunningRequests == 3);
        }
        [TestMethod]
        public void TenCalls()
        {
            Stopwatch sw = new Stopwatch();

            JsonReader bioreader = new JsonReader();

            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
          
            sw.Start();
            bioreader.ReadDataAsync(10);
            while (breathingNode.CallCount < 10) ;
            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 30000);
            Assert.AreEqual(breathingNode.CallCount, 10);
        }
        [TestMethod]
        public async Task ContinualReading()
        {
            Stopwatch sw = new Stopwatch();

            JsonReader bioreader = new JsonReader();

            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);

            bioreader.StartContinualReading();
            await Task.Delay(5000);
            bioreader.StopContinualReading();
            await Task.Delay(1000);
            Assert.IsTrue(bioreader.RunningRequests == 0);
            
        }
        [TestMethod]
        public async Task ReadingFakePath()
        {
            Stopwatch sw = new Stopwatch();

            JsonReader bioreader = new JsonReader();

            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>(@"http://fakemamucz.cz", bioreader, jsonWebReader, jsonDataDeserealizer);

            bioreader.ReadDataAsync();
            await Task.Delay(5000);            
            Assert.IsTrue(bioreader.RunningRequests == 0);

        }
        [TestMethod]
        public void WriteToSql()
        {
            var data = new BreathingDataProvider.RequiredFormat();
            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider("DELLPETR", "SQLEXPRESS", "mamucz", "Merlin2495", "BSDDATA");
            SqlRepository<BreathingDataProvider.RequiredFormat> sqlRep = new SqlRepository<BreathingDataProvider.RequiredFormat>(sqlp);
            sqlRep.Insert(data);
        }
        [TestMethod]
        public void ReadJsoBreathing10WriteSql()
        {
            JsonReader bioreader = new JsonReader();

            //READ BREATH
            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>( breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);

            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider("DELLPETR", "SQLEXPRESS", "mamucz", "Merlin2495", "BSDDATA");
            SqlRepository<BreathingDataProvider.RequiredFormat> sqlRepRequiredFormat = new SqlRepository<BreathingDataProvider.RequiredFormat>(sqlp);
            SqlRepository<BreathingDataProvider.DataframeRow> sqlRepBreathPeeks = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathPeekData");
            SqlRepository<BreathingDataProvider.DataframeRow> sqlRepBreathRaw = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathRawData");
            
            double lastRawtimems = 0;
            double lastPeektimems = 0;
            for (int j=0;j<10;j++)
            { 
                var data = breathingNode.ReadData();
                Assert.IsNotNull(data);
                var peeks = new BreathingDataProvider.DataframeToRows(data.peakData.dataframe,j,0,0);
                var raws = new BreathingDataProvider.DataframeToRows(data.rawData.dataframe,j,0,0);
                
                sqlRepRequiredFormat.Insert(data.requiredFormat);

                var newraws = raws.Where(x => x.timems > lastRawtimems).ToArray();
                var newpeeks = peeks.Where(x => x.timems > lastPeektimems).ToArray();
                if (newpeeks.Length != 0)
                    lastPeektimems = newpeeks.Max(x => x.timems);
                if (newraws.Length != 0)
                    lastRawtimems = newraws.Max(x => x.timems);
                if (j > 0)
                { 
                    for (int i = 0; i < newpeeks.Count();i++)
                    {
                        newpeeks[i].SessionID = j;
                        sqlRepBreathPeeks.Insert(newpeeks[i]);
                    }
                }

                if (j > 0)
                { 
                    for (int i = 0; i < newraws.Count(); i++)
                    {
                        newraws[i].SessionID = j;
                        sqlRepBreathRaw.Insert(newraws[i]);
                    }
                }
            }
            sqlRepRequiredFormat.Dispose();
            sqlRepBreathPeeks.Dispose();
            sqlRepBreathRaw.Dispose();


        }
        [TestMethod]
        public void ReadJsoBreathing10()
        {
            JsonReader bioreader = new JsonReader();

            //READ BREATH
            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var breathingNode = bioreader.AddReader<BreathingDataProvider.Root>(breathServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);

            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider("DELLPETR", "SQLEXPRESS", "mamucz", "Merlin2495", "BSDDATA");
            SqlRepository<BreathingDataProvider.RequiredFormat> sqlRepRequiredFormat = new SqlRepository<BreathingDataProvider.RequiredFormat>(sqlp);
            SqlRepository<BreathingDataProvider.DataframeRow> sqlRepBreathPeeks = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathPeekData");
            SqlRepository<BreathingDataProvider.DataframeRow> sqlRepBreathRaw = new SqlRepository<BreathingDataProvider.DataframeRow>(sqlp, "SimSessionBioBrathRawData");

            double lastRawtimems = 0;
            double lastPeektimems = 0;
            for (int j = 0; j < 10; j++)
            {
                var data = breathingNode.ReadData();
                Assert.IsNotNull(data);
                var peeks = new BreathingDataProvider.DataframeToRows(data.peakData.dataframe, j, 0, 0);
                var raws = new BreathingDataProvider.DataframeToRows(data.rawData.dataframe, j, 0, 0);
                //sqlRepRequiredFormat.Insert(data.requiredFormat);

                var newraws = raws.Where(x => x.timems > lastRawtimems).ToArray();
                var newpeeks = peeks.Where(x => x.timems > lastPeektimems).ToArray();
                if (newpeeks.Length != 0)
                    lastPeektimems = newpeeks.Max(x => x.timems);
                if (newraws.Length != 0)
                    lastRawtimems = newraws.Max(x => x.timems);
                if (j > 0)
                {
                    for (int i = 0; i < newpeeks.Count(); i++)
                    {
                        newpeeks[i].SessionID = j;
                        //sqlRepBreathPeeks.Insert(newpeeks[i]);
                    }
                }

                if (j > 0)
                {
                    for (int i = 0; i < newraws.Count(); i++)
                    {
                        newraws[i].SessionID = j;
                        //sqlRepBreathRaw.Insert(newraws[i]);
                    }
                }

            }
            sqlRepRequiredFormat.Dispose();
            sqlRepBreathPeeks.Dispose();
            sqlRepBreathRaw.Dispose();


        }
        [TestMethod]
        public void ReadJsoHeartWriteSql()
        {
            JsonReader bioreader = new JsonReader();

            //READ BREATH
            JsonGetDataReder jsonWebReader = new JsonGetDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var heartNode = bioreader.AddReader<HeartDataProvider.Root>(heartServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            var dataHeart = heartNode.ReadData();

            Assert.IsNotNull(dataHeart);
            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider("DELLPETR", "SQLEXPRESS", "mamucz", "Merlin2495", "BSDDATA");
            SqlRepository<HeartDataProvider.Root> sqlRepRequiredFormat = new SqlRepository<HeartDataProvider.Root>(sqlp);
            sqlRepRequiredFormat.Insert(dataHeart);
            sqlRepRequiredFormat.Dispose();                   
        }
        [TestMethod]
        public void ReadJsoEmotionsWriteSql()
        {
            JsonReader bioreader = new JsonReader();

            //READ BREATH
            JsonWebDataReder jsonWebReader = new JsonWebDataReder();
            JsonDataDeserealizer jsonDataDeserealizer = new JsonDataDeserealizer();
            var emotionNode = bioreader.AddReader<EmotionDataProvider.Root>(emotionsServerURL, bioreader, jsonWebReader, jsonDataDeserealizer);
            var dataEmotion = emotionNode.ReadData();

            Assert.IsNotNull(dataEmotion);
            ISqlConnectionStringProvider sqlp = new ConnectionStringProvider("DELLPETR", "SQLEXPRESS", "mamucz", "Merlin2495", "BSDDATA");
            SqlRepository<EmotionDataProvider.EmotionsSql> sqlRepRequiredFormat = new SqlRepository<EmotionDataProvider.EmotionsSql>(sqlp);
            sqlRepRequiredFormat.Insert(new EmotionDataProvider.EmotionsSql(dataEmotion));
            sqlRepRequiredFormat.Dispose();
        }
        private void Breathing_DataReady(object sender, BreathingDataProvider.Root args)
        {
            Assert.AreNotEqual(0, args.requiredFormat.UTS);
        }
    }
}
