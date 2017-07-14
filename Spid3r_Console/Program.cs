using System;
using Spid3r_Console.Log;
using Spid3r_Console.Spid3r;
using Spid3r_Console.Database;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace Spid3r_Console
{
    public class Spid3rConsole
    {
        private static ILogger _logger = LogManager.GetLogger(typeof(Spid3rConsole));
        private static ConcurrentBag<ScrapedData> _dataBag = new ConcurrentBag<ScrapedData>();
        private static List<Spid3r.Spid3r> _spiderList = new List<Spid3r.Spid3r>();
        static void Main(string[] args)
        {
            LogManager.WriteToFile = false;
            _logger.Log("Spide3r - V0.1");
            //var spider = new CTSpid3r(Properties.Resources.Home_All, new Adapter());
            var spider = new MBRVSpid3r(Properties.Resources.MBRV_HCM, 2, new Adapter());
            _spiderList.Add(spider);
            var dataThread = new Thread(GetDataThread);
            dataThread.Start();
            for (int i = 0; i < 1000; i++)
            {
                _logger.Log(string.Format("amount of data: {0}", _dataBag.Count));
                Thread.Sleep(100);
            }
            Console.ReadLine();
        }
        private static void GetDataThread()
        {
            foreach (var spider in _spiderList)
            {
                spider.GetData(1, 7, ref _dataBag);
            }
        }
    }
}
