using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using WebDiff;
using System.Net.Http;

namespace CheckerUnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task LocalArchiveNotDefinitionAsync()
        {
            var answer = false;
            Checker checker = new Checker(new Uri("https://example.com/"));
            try
            {
                await checker.CheckAsync();
            }catch(NullReferenceException)
            {
                answer = true;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public async Task ServerNotFoundAsync()
        {
            var answer = false;
            Checker checker = new Checker(new Uri("http://example.jp/"), @"D:\ArchiveLog.txt");
            try
            {
                await checker.GetAndSavePageAsync();
            }
            catch (HttpRequestException)
            {
                answer = true;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public async Task ArchiveSaveAsync()
        {
            var answer = true;
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            try
            {
                await checker.GetAndSavePageAsync();
            }catch(Exception)
            {
                answer = false;
            }
            Assert.AreEqual(expected: true, actual: answer);
        }

        [TestMethod]
        public async Task ArchiveCheckAsync()
        {
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            await checker.GetAndSavePageAsync();
            Checker checker2 = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            var answer = await checker2.CheckAsync();
            Assert.AreEqual(expected: 1.0, actual: answer);
        }

        [TestMethod]
        public async Task ArchiveCannotCheckAsync()
        {
            Checker checker = new Checker(new Uri("https://example.com/"), @"D:\ArchiveLog.txt");
            await checker.GetAndSavePageAsync();
            Checker checker2 = new Checker(new Uri("https://www.google.com/"), @"D:\ArchiveLog.txt");
            var answer = await checker2.CheckAsync();
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

