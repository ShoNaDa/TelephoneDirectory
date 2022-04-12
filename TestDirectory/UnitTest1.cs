using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TelephoneDirectory;

namespace TestDirectory
{
    [TestClass]
    public class UnitTest1
    {
        readonly MainWindow mw = new MainWindow();

        [TestMethod]
        public void AddOrgTest()
        {
            Assert.IsFalse(mw.AddToDB("TTIT", "Номер", "Герцена 18", "08:00 - 22:00", "IT"));
            Assert.IsFalse(mw.AddToDB("", "", "", "", ""));
            Assert.IsFalse(mw.AddToDB("", "Номер", "Герцена 18", "08:00 - 22:00", " "));

            Assert.IsTrue(mw.AddToDB("TTIT", "89539152059", "Герцена 18", "08:00 - 22:00", "IT"));
        }

        [TestMethod]
        public void EditOrgTest()
        {
            Assert.IsFalse(mw.EditToDB("TTIT", "Номер", "Герцена 18", "08:00 - 22:00", "IT", 120));
            Assert.IsFalse(mw.EditToDB("", "", "", "", "", 120));
            Assert.IsFalse(mw.EditToDB("TTIT", "+7(952) 915 20 59", "Герцена 18", "08:00 - 22:00", "IT", 120));

            Assert.IsTrue(mw.EditToDB("TTIT", "100200", "Герцена 18", "08:00 - 22:00", "IT", 120));
        }

        [TestMethod]
        public void DeleteOrgTest()
        {
            Assert.IsTrue(mw.DeleteToDB(120));
        }

        [TestMethod]
        public void SortTest()
        {
            List<string> expected = new List<string> { "IT", "IT", "Сфера деятельности" };
            List<string> actual = new List<string>();

            List<Directory> list = mw.SortList();

            foreach (var item in list)
            {
                actual.Add(item.sphereActivity.ToString());
            }

            for(int i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }

            expected = new List<string> { "IT", "Сфера деятельности", "IT" };

            Assert.AreNotEqual(expected[1], actual[1]);
            Assert.AreNotEqual(expected[2], actual[2]);
        }

        [TestMethod]
        public void NumberTest()
        {
            Assert.IsTrue(mw.CheckingForNumbers("123"));

            Assert.IsFalse(mw.CheckingForNumbers(""));
            Assert.IsFalse(mw.CheckingForNumbers("dfh"));
        }

        [TestMethod]
        public void StringTest()
        {
            Assert.IsTrue(mw.StringNotEmpty("TIT"));

            Assert.IsFalse(mw.CheckingForNumbers(""));
            Assert.IsFalse(mw.CheckingForNumbers(" "));
            Assert.IsFalse(mw.CheckingForNumbers("     "));
        }
    }
}