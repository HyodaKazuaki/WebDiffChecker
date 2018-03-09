using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using F23.StringSimilarity;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebDiff
{
    public class Checker
    {
        private static Uri SiteUri;
        private static string LocalPath;
        private static WebClient webClient;

        /// <summary>
        /// Webページの内容を差分し一致率を調べます。
        /// </summary>
        /// <param name="siteUrl">WebページのURL</param>
        /// <param name="localPath">Webページのローカルアーカイブパス</param>
        public Checker(Uri siteUrl, string localPath = null)
        {
            if (!(localPath is null) && !Path.IsPathRooted(localPath))
                throw new ArgumentException("Local Archive path is illegal");

            SiteUri = siteUrl;
            LocalPath = localPath;
        }
        
        /// <summary>
        /// Webページへアクセスしローカルアーカイブを作成します。
        /// </summary>
        public void GetAndSavePage()
        {
            SaveArchive(GetWebPage());
        }

        /// <summary>
        /// Webページへアクセスします。
        /// </summary>
        /// <returns>ローカルアーカイブのデータ</returns>
        private string GetWebPage()
        {
            using (webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                return ParseBody(webClient.DownloadString(SiteUri));
            }
        }

        /// <summary>
        /// WebページからBody要素のみを抽出します。
        /// </summary>
        /// <param name="webSite">Webページのデータ</param>
        /// <returns>Body要素</returns>
        private string ParseBody(string webSite)
        {
            HtmlParser htmlParser = new HtmlParser();
            var doc = htmlParser.Parse(webSite).QuerySelector("body");
            return doc.Text();
        }

        /// <summary>
        /// Webページのローカルアーカイブを作成します。
        /// </summary>
        private void SaveArchive(string body)
        {
            checkArchiveDefinition();
            using (StreamWriter writer = new StreamWriter(LocalPath))
            {
                writer.Write(body);
            }
        }

        /// <summary>
        /// Webページの一致率を調べます。
        /// </summary>
        /// <returns>一致率(%)</returns>
        public double Check()
        {
            string archive = ReadArchive();
            string webpage = GetWebPage();
            JaroWinkler jaroWinkler = new JaroWinkler();

            return jaroWinkler.Similarity(archive, webpage);
        }

        /// <summary>
        /// ローカルアーカイブを取得します。
        /// </summary>
        /// <returns>ローカルアーカイブのデータ</returns>
        private string ReadArchive()
        {
            checkArchiveDefinition();
            using (StreamReader streamReader = new StreamReader(LocalPath))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// ローカルアーカイブのパスが存在するか確認します。
        /// </summary>
        private void checkArchiveDefinition()
        {
            if (LocalPath is null) throw new NullReferenceException("Local Archive Path not Definition");
        }
    }
}
