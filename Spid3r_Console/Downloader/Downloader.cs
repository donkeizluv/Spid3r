using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Spid3r_Console.Downloader
{
    public interface IDownloader
    {
        Task<Stream> OpenReadAsync(string url);
        Task<Stream> DownloadDataAsync(string url);
        event DownloadProgressChangedEventHandler ProgressChanged;
    }

    public class Downloader : IDownloader
    {
        public async Task<Stream> OpenReadAsync(string url)
        {
            using (var wd = new WebDownload())
            {
                return await wd.OpenReadTaskAsync(HandleWeirdFormat(url));
            }
        }
        private static string HandleWeirdFormat(string url)
        {
            if (url.StartsWith("/"))
                return "http://" + url.TrimStart('/');
            if (!url.StartsWith("http"))
                return "http://" + url;
            return url;
        }
        /// <summary>
        ///     Use this for progress track support
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadDataAsync(string url)
        {
            using (var wd = new WebDownload())
            {
                wd.DownloadProgressChanged += (s, e) => { ProgressChanged?.Invoke(this, e); };
                return new MemoryStream(await wd.DownloadDataTaskAsync(HandleWeirdFormat(url)));
            }
        }

        public event DownloadProgressChangedEventHandler ProgressChanged;
    }
    public class WebDownload : WebClient
    {
        private const int timeOut = 10000;

        public WebDownload()
        {
            Timeout = timeOut;
            Headers[HttpRequestHeader.UserAgent] =
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/48.0.2564.103 Safari/535.2";
        }

        /// <summary>
        ///     Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (request == null) return null;
            request.Referer = address.AbsoluteUri; //solved wallpaperwide anti crawler
            request.Timeout = Timeout;
            request.Proxy = null;
            //request.KeepAlive = false; // keep conns count low but reduce too much speed
            return request;
        }
    }
}