using System;

namespace Spid3r_Console.Log
{
    public interface ILogger
    {
        Type ClassType { get; }
        void Log(string log);
        void Log(Exception ex);
        event OnNewLogHandler OnNewLog;
    }
}