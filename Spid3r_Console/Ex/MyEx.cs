using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spid3r_Console.Ex
{
    public class ContentEndException : Exception
    {
        public string HTMLRespone { get; set; }
        public ContentEndException(string message) : base(message)
        {
        }
        public ContentEndException(string message, Exception inner) : base(message, inner)
        {
        }
        public ContentEndException(string message, string html) : base(message)
        {
            HTMLRespone = html;
        }
    }
}
