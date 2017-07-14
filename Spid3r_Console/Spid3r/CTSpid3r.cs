//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Spid3r_Console.Database;
//using Spid3r_Console.Downloader;
//using Spid3r_Console.Log;
//using Spid3r_Console.Downloader;
//using Spid3r_Console.Helper;
//using HtmlAgilityPack;
//using System.IO;

//namespace Spid3r_Console.Spid3r
//{
//    /// <summary>
//    /// chotot.com spider, this page has encrypted content. this class doesnt work yet.
//    /// </summary>
//    public class CTSpid3r : Spid3r
//    {
//        //ex: @"//span[@class='btn btn-success download-button']";
//        private const string DataLinksPath = @"//div[@class='row.no-margin']/div/div/div/ul/li/a";
//        public CTSpid3r(string seed, IDbAdapter adapter) : base(seed, adapter)
//        {
//            Logger = LogManager.GetLogger(this.GetType());
//            Downloader = new Downloader.Downloader();
//        }    
//        protected override IDownloader Downloader { get; }
//        protected override ILogger Logger { get; }
//        protected override List<string> ParseForDataLinks(string html)
//        {
//            var datalinks = new List<string>();
//            var doc = new HtmlDocument();
//            doc.Load(html);
//            var nodes = doc.DocumentNode.SelectNodes(DataLinksPath);
//            foreach (var node in nodes)
//            {
//                datalinks.Add(Home + node.Attributes["href"]);
//            }
//            return datalinks;
//        }
//        //public void Test()
//        //{
//        //    CurrentPageNumber = 1;
//        //    MaxThreads = 4;
//        //    FetchMoreDatalinks();
//        //}

//        //protected async Task<bool> FetchMoreDatalinks()
//        //{
//        //    try
//        //    {
//        //        Datalinks = new ConcurrentBag<string>(Datalinks.Concat(await GetDataLinks(CurrentPageNumber, CurrentPageNumber + MaxThreads).ConfigureAwait(false)));
//        //        return true;
//        //    }
//        //    catch (Exception ex) //be more specific
//        //    {
//        //        Logger.Log(ex);
//        //        return false;
//        //    }
//        //}
//    }
//}
