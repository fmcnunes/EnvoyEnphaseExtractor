using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using static RequestEnvoyCounters.Program;

namespace RequestEnvoyCounters
{
    class Config
    {
        private Logger logger = new Logger("Config");
        private object oLock = new object();
        private static dynamic jsonObj = null;


        public Config()
        {
            lock(oLock)
            {
                if (jsonObj ==  null)
                {
                    String filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    string json = File.ReadAllText(filePath);
                    jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                }
            }
        }

        public String GetString(String name)
        {
            try
            {
                return (String) jsonObj[name];
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "Config", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
            return null;
        }

        public int GetInt(String name)
        {
            try
            {
                return (int) jsonObj[name];
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "Config", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
            return -1;
        }

        public String GetString(String section, String name)
        {
            try
            {
                return (String) jsonObj[section][name];
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "Config", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
            return "";
        }

        public int GetInt(String section, String name)
        {
            try
            {
                return (int) jsonObj[section][name];
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "Config", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
            return -1;
        }
    }
}

