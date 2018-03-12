using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using F23.StringSimilarity;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebDiff
{
    public class Checker
    {
        private static Uri SiteUri;
        private static string LocalPath;
        private static HttpClient httpClient = new HttpClient();

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
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299");
        }

        /// <summary>
        /// Webページへアクセスしローカルアーカイブを作成します。
        /// </summary>
        public async Task GetAndSavePageAsync()
        {
            await SaveArchiveAsync(await GetWebPageAsync());
        }

        /// <summary>
        /// Webページへアクセスします。
        /// </summary>
        /// <returns>ローカルアーカイブのデータ</returns>
        private async Task<string> GetWebPageAsync()
        {
            return ParseBody(await httpClient.GetStringAsync(SiteUri));
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
        private async Task SaveArchiveAsync(string body)
        {
            checkArchiveDefinition();
            using (StreamWriter writer = new StreamWriter(LocalPath))
            {
                await writer.WriteAsync(body);
            }
        }

        /// <summary>
        /// Webページの一致率を調べます。
        /// </summary>
        /// <returns>一致率(%)</returns>
        public async Task<double> CheckAsync()
        {
            string archive = await ReadArchiveAsync();
            string webpage = await GetWebPageAsync();
            JaroWinkler jaroWinkler = new JaroWinkler();

            return jaroWinkler.Similarity(archive, webpage);
        }

        /// <summary>
        /// ローカルアーカイブを取得します。
        /// </summary>
        /// <returns>ローカルアーカイブのデータ</returns>
        private async Task<string> ReadArchiveAsync()
        {
            checkArchiveDefinition();
            using (StreamReader streamReader = new StreamReader(LocalPath))
            {
                return await streamReader.ReadToEndAsync();
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
