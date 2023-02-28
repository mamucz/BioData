using logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTP.BioToSQL;
using System.Xml.Linq;

namespace BiometricDataJsonTest
{
    [TestClass]
    public class BioToSQLTest
    {
        [TestMethod]
        public void TestCreateSaveAndRestore()
        {
            BioToSQL test = new BioToSQL();
            test.breathServerURL = @"http://192.168.168.4:8082/hrvparams";
            test.heartServerURL = @"http://192.168.168.4:8081/hrvparams";
            test.emotionsServerURL = @"http://192.168.168.4:5000/face-tracker";
          
            test.SqlServerName = "DELLPETR";
            test.SqlInstanceName = "SQLEXPRESS";
            test.UserName = "mamucz";
            test.Password = "Merlin2495";
            test.Database = "BSDDATA";

            var xdata = test.GetCustomXElements();
            BioToSQL test2 = new BioToSQL();
            XElement root = new XElement("root",xdata);
            test2.FromCustomXElements(root);

            Assert.AreEqual(test.breathServerURL, test2.breathServerURL);
            Assert.AreEqual(test.heartServerURL, test2.heartServerURL);
            Assert.AreEqual(test.emotionsServerURL, test2.emotionsServerURL);

            Assert.AreEqual(test.SqlServerName, test2.SqlServerName);
            Assert.AreEqual(test.SqlInstanceName, test2.SqlInstanceName);
            Assert.AreEqual(test.UserName, test2.UserName);
            Assert.AreEqual(test.Password, test2.Password);
            Assert.AreEqual(test.Database, test2.Database);
        }
        [TestMethod]
        public void StartParadigma()
        {
            BioToSQL test = new BioToSQL();
            test.breathServerURL = @"http://192.168.168.4:8082/hrvparams";
            test.heartServerURL = @"http://192.168.168.4:8081/hrvparams";
            test.emotionsServerURL = @"http://192.168.168.4:5000/face-tracker";

            test.SqlServerName = "DELLPETR";
            test.SqlInstanceName = "SQLEXPRESS";
            test.UserName = "mamucz";
            test.Password = "Merlin2495";
            test.Database = "BSDDATA";

            Constant CSessionID = new Constant();
            Constant CTIME = new Constant();
            Constant CPhase = new Constant();
            Constant CEnable = new Constant();
            Constant CError = new Constant();

            test.SessionID.SetElement(CSessionID);
            test.TIME.SetElement(CTIME);
            test.Phase.SetElement(CPhase);
            test.Enable.SetElement(CEnable);
            test.Error.SetElement(CError);

            CSessionID.SetValue(1);
            CTIME.SetValue(0.5);
            CPhase.SetValue(1);
            CEnable.SetValue(1);

            test.Prepare(new Theorem("test"));
        }
    }
}
