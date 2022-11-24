using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Repository;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace RequestEnvoyCounters
{
    public class Logger
    {
        public enum Level { Info, Debug, Warn, Error, Fatal };
        private static ILog log = null;
        private static object oLock = new object();
        public Logger(string enuv)
        {
            lock (oLock)
            {
                if (log == null)
                {
                    try
                    {
                        ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                        XmlConfigurator.Configure(logRepository, new FileInfo("log4netconfig.config"));
                        log = LogManager.GetLogger(enuv);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FATAL ERROR, Logger -Fatal Error: " + ex.ToString());
                    }
                }
            }
        }
        public void Log(Level level, string LogScope, string msg)
        {
            switch (level)
            {
                case Level.Info: log.Info(LogScope + "\t" + msg); break;
                case Level.Debug: log.Debug(LogScope + "\t" + msg); break;
                case Level.Warn: log.Warn(LogScope + "\t" + msg); break;
                case Level.Error: log.Error(LogScope + "\t" + msg); break;
                default: log.Fatal(LogScope + "\t" + msg); break;
            }
        }
    }
}
