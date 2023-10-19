using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Diagnostics;

namespace RequestEnvoyCounters
{
    internal class Envoy
    {
        InfluxDb influxDb = new InfluxDb();

        Logger log = new Logger("Envoy");

        private static String url = "";

        public Envoy()
        {
            Config config = new Config();

            url =      config.GetString("Envoy", "url");
			
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        }

        public void Run()
        {
            Thread t = new Thread(new ThreadStart(LoopInverters));
            t.Start();
            t = new Thread(new ThreadStart(LoopMeeters));
            t.Start();
        }

        private void LoopInverters()
        {
            Stopwatch sw = new Stopwatch();

            while (true)
            {
                try
                {
                    sw.Reset();
                    sw.Start();

                    ReadInverters();

                    sw.Stop();
                    if (sw.ElapsedMilliseconds >= 15000)
                    {
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        Thread.Sleep(15000 - (int)sw.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    log.Log(Logger.Level.Error, "LoopMeeters", ex.Message);
                    Thread.Sleep(15000);
                }
            }
        }

        private void LoopMeeters()
        {
            Stopwatch sw = new Stopwatch();

            while (true)
            {
                try
                {
                    sw.Reset();
                    sw.Start();

                    ReadMeters();

                    sw.Stop();
                    if (sw.ElapsedMilliseconds >= 15000)
                    {
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        Thread.Sleep(15000 - (int)sw.ElapsedMilliseconds);
                    }
                }
                catch(Exception ex)
                {
                    log.Log(Logger.Level.Error, "LoopMeeters", ex.Message);
                    Thread.Sleep(15000);
                }
            }
        }

        /*
         http://192.168.0.118/ivp/meters/readings
        */


        private void ReadInverters()
        {
            String responseString = RequestData($"{url}/api/v1/production/inverters");

            List<InverterReadings> inverterReadings = JsonConvert.DeserializeObject<List<InverterReadings>>(responseString);

            log.Log(Logger.Level.Info, "ReadInverters", $"Received {inverterReadings.Count} metrics.");

            influxDb.StoreInverterReadings(inverterReadings);
        }

        private void ReadMeters()
        {
            String responseString = RequestData($"{url}/ivp/meters/readings");

            List<MeterReading> meterReadings = JsonConvert.DeserializeObject<List<MeterReading>>(responseString);

            log.Log(Logger.Level.Info, "ReadMeters", $"Received {meterReadings.Count} metrics.");

            influxDb.StoreMeterReadings(meterReadings);
        }

        private String RequestData(String url)
        {
            Uri uri = new Uri(url);
            WebRequest request = WebRequest.Create(uri);

            request.Headers.Add("Authorization", $"Bearer {token}");
            log.Log(Logger.Level.Info, "ReadMeters", $"Bearer {token}");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
    }
}
