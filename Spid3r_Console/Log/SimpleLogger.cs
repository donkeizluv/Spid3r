using System;
using System.Text;

namespace Spid3r_Console.Log
{
    public class SimpleLogger : ILogger
    {
        public SimpleLogger(Type type)
        {
            ClassType = type;
        }

        public Type ClassType { get; }
        public event OnNewLogHandler OnNewLog;

        public void Log(Exception ex)
        {
            var builder = new StringBuilder();
            builder.Append(ex.Message).Append(ex.StackTrace);
            if(ex.InnerException != null)
            {
                builder.Append(ex.InnerException.Message).Append(ex.InnerException.StackTrace);
            }
            RaiseNewLogEvent(builder.ToString());
        }

        public void Log(string log)
        {
            RaiseNewLogEvent(log);
        }

        private void RaiseNewLogEvent(string log)
        {
            OnNewLog?.Invoke(this, new NewLogEventArgs(log));
        }
    }
}