using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spid3r_Console.Database;
using Spid3r_Console.Downloader;
using Spid3r_Console.Log;
using System.Collections.Concurrent;
using HtmlAgilityPack;
using System.IO;
using Spid3r_Console.Helper;
using Spid3r_Console.Ex;
using System.Diagnostics;

namespace Spid3r_Console.Spid3r
{
    public class MBRVSpid3r : Spid3r
    {
        //xpath
        private const string DataLinksPath = @"//a[@class='record_1']";
        private const string DataPath = @"//td[@class='detail_mess']";


        public MBRVSpid3r(string seed, int maxThreads, IDbAdapter adapter) : base(seed, maxThreads, adapter)
        {
            Logger = LogManager.GetLogger(this.GetType());
            Downloader = new Downloader.Downloader();
        }
        protected override IDownloader Downloader { get; }
        protected override ILogger Logger { get; }
        protected override List<string> ParseForDataUrls(string html)
        {
            var datalinks = new List<string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(DataLinksPath);
            if (nodes.Count < 1) throw new ContentEndException("Cant parse more data links, maybe end or restricted", html);
            foreach (var node in nodes)
            {
                var dataLink = Home + node.Attributes["href"].Value.ToString();
                //if (Datalinks.Contains(dataLink)) throw new InvalidDataException("this line already in the list!");
                datalinks.Add(dataLink);
            }
            return datalinks;
        }
        protected override List<ScrapedData> ParseForData(string html)
        {
            var data = new List<ScrapedData>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(DataPath);
            if (nodes.Count < 1)
            {
                Logger.Log("no data in path -> skip");
                return null;
            }
            foreach (var node in nodes)
            {
                Debug.Print(node.InnerText);
            }
            return data;
        }
        public void Test()
        {
            CurrentPageNumber = 1;
            MaxThreads = 3;
            //FetchMoreDatalinks(); //ok
        }
    }
}
