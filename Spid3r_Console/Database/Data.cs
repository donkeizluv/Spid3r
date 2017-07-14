using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spid3r_Console.Database
{
    public class ScrapedData
    {
        public string Tittle { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public ScrapedData()
        {

        }
        public ScrapedData(string title, string name, string phone, string location)
        {
            Tittle = title;
            Name = name;
            Phone = phone;
            Location = location;
        }
    }
}
