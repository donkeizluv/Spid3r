using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using Spid3r_Console.Ex;
using Spid3r_Console.Database;
using Spid3r_Console.Log;
using Spid3r_Console.Downloader;
using Spid3r_Console.Helper;

namespace Spid3r_Console.Spid3r
{
    public abstract class Spid3r
    {
        protected abstract IDownloader Downloader { get; }
        protected abstract ILogger Logger { get; }

        public ConcurrentQueue<string> Datalinks = new ConcurrentQueue<string>();
        public int MaxThreads { get; set; }
        public int CurrentPageNumber { get; protected set; }
        public string SeedUrl { get; private set; }
        public string Home { get; private set; }
        public bool ContentEnd { get; private set; } = false;
        public IDbAdapter Adapter { get; private set; }
        public Spid3r(string seed, int maxThreads, IDbAdapter adapter)
        {
            SeedUrl = seed;
            Adapter = adapter;
            Home = ExtractHome(seed);
            MaxThreads = maxThreads;
        }
        protected abstract List<string> ParseForDataUrls(string html);
        protected abstract List<ScrapedData> ParseForData(string html);

        /// <summary>
        /// extract home fron url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual string ExtractHome(string url)
        {
            var uri = new Uri(url);
            return uri.Scheme + "://" + uri.Authority;
        }
        protected virtual string GetNextLink()
        {
            CurrentPageNumber++;
            return SeedUrl.Replace("@", (CurrentPageNumber - 1).ToString());
        }
        /// <summary>
        /// Begin scraping data
        /// </summary>
        /// <param name="startPage">page to start from</param>
        /// <param name="dataLimit">number of data to scrape then stop, -1 to scrape till nothing left</param>
        /// <returns></returns>
        public virtual void GetData(int startPage, int dataLimit, ref ConcurrentBag<ScrapedData> dataBag) //non sense, totally wrong
        {
            //CurrentPageNumber = startPage;
            //for (int i = CurrentPageNumber; (i <= dataLimit) || (dataLimit == -1 && !ContentEnd);)
            //{
            //    if (Datalinks.Count < MaxThreads)
            //    {
            //        Task.WaitAll(FetchMoreDatalinks());
            //        i = CurrentPageNumber; //update current page
            //    }
            //    var taskList = new List<Task<Stream>>();
            //    for (int thread = 0; thread < MaxThreads; thread++)
            //    {
            //        string datalink;
            //        if (!Datalinks.TryDequeue(out datalink)) throw new InvalidDataException("Datalinks is empty.");
            //        var task = Downloader.OpenReadAsync(datalink);
            //        taskList.Add(task);
            //        i++;
            //    }
            //    foreach (var result in GetDataAsync(taskList).Result)
            //    {
            //        dataBag.Add(result);
            //    }
            //}

            int dataCount = 0;
            while (!ContentEnd && dataCount <= dataLimit)
            {
                if (Datalinks.Count < MaxThreads)
                {
                    Task.WaitAll(FetchMoreDatalinks());
                }
                var taskList = new List<Task<Stream>>();
                for (int thread = 0; thread < MaxThreads; thread++)
                {
                    if (!Datalinks.TryDequeue(out string datalink)) throw new InvalidDataException("Datalinks is empty.");
                    var task = Downloader.OpenReadAsync(datalink);
                    taskList.Add(task);
                }
                foreach (var result in GetDataAsync(taskList).Result)
                {
                    dataBag.Add(result);
                    dataCount++;
                }

            }
        }
        protected virtual async Task<ConcurrentBag<ScrapedData>> GetDataAsync(List<Task<Stream>> taskList)
        {
            var dataBag = new ConcurrentBag<ScrapedData>();
            foreach (var task in taskList)
            {
                foreach (var item in ParseForData((await task.ConfigureAwait(false)).ConvertToString()))
                {
                    dataBag.Add(item);
                }
            }
            await Task.WhenAll(taskList);
            return dataBag;
        }
        protected virtual async Task<bool> FetchMoreDatalinks()
        {
            try
            {
                foreach (var item in await GetDataUrls(CurrentPageNumber, CurrentPageNumber + MaxThreads).ConfigureAwait(false))
                {
                    Datalinks.Enqueue(item);
                }
                return true;
            }
            catch (Exception ex) //be more specific
            {
                Logger.Log(ex);
                return false;
            }
        }
        protected virtual async Task<ConcurrentQueue<string>> GetDataUrls(int fromPage, int toPage)
        {
            //Logger.Log(string.Format("Start downloading data links from {0} to {1}", fromPage, toPage));
            var dataUrls = new ConcurrentQueue<string>();
            int i;
            for (i = fromPage; i <= toPage;)
            {
                var taskList = new List<Task<Stream>>();
                for (int thread = 0; thread < MaxThreads && i <= toPage; thread++)
                {
                    string url = GetNextLink(); //getnextlink will update currentpage
                    Logger.Log(string.Format("Scraping data urls: {0}", url));
                    var task = Downloader.OpenReadAsync(url);
                    taskList.Add(task);
                    i++;
                }
                //loop all results
                foreach (var task in taskList)
                {
                    try
                    {
                        //add results to bag
                        foreach (var item in ParseForDataUrls((await task.ConfigureAwait(false)).ConvertToString()))
                        {
                            dataUrls.Enqueue(item);
                        }
                    }
                    catch (ContentEndException)
                    {
                        Logger.Log("Content end or restricted -> set flag");
                        ContentEnd = true;
                        //throw;
                    }
                }
                await Task.WhenAll(taskList);
            }
            Logger.Log(string.Format("Download data links completed, urls: {0}", dataUrls.Count));
            return dataUrls;
        }
        //protected abstract bool FetchMoreDatalinks();
        //protected abstract ConcurrentBag<string> GetDataLinks(int fromPage, int toPage); //leave this to the implemented

    }
}
