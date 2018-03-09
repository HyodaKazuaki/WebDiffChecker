using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using WebDiff;

namespace CheckerUnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LocalArchiveNotDefinition()
        {
            var answer = false;
            Checker checker = new Checker(new Uri("https://example.com/"));
            try
            {
                checker.Check();
            }catch(NullReferenceException)
            {
                answer = true;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public void ServerNotFound()
        {
            var answer = false;
            Checker checker = new Checker(new Uri("http://example.jp/"), @"D:\ArchiveLog.txt");
            try
            {
                checker.GetAndSavePage();
            }
            catch (WebException)
            {
                answer = true;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public void ArchiveSave()
        {
            var answer = true;
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            try
            {
                checker.GetAndSavePage();
            }catch(Exception)
            {
                answer = false;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public void ArchiveCheck()
        {
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            checker.GetAndSavePage();
            var answer = checker.Check();
            Assert.AreEqual(expected: 1.0, actual: answer);
        }

        [TestMethod]
        public void ArchiveCannotCheck()
        {
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            checker.GetAndSavePage();
            Checker checker2 = new Checker(new Uri("https://www.google.com/"), @"D:\ArchiveLog.txt");
            var answer = checker2.Check();
            Assert.AreNotEqual(notExpected: 1.0, actual: answer);
        }

        [TestMethod]
        public void LocalArchivePathIsIllegal()
        {
            var answer = false;
            try
            {
                Checker checker = new Checker(new Uri("https://example.com/"), @"jgbrigjj");
            }
            catch (ArgumentException)
            {
                answer = true;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }
    }
}

