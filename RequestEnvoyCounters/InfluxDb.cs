using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RequestEnvoyCounters
{
    class InfluxDb
    {
        private Logger logger = new Logger("InfluxDb");

        private String influxDbHost;
        private int influxDbPort;
        private String influxDb;

        public InfluxDb()
        {
            Config config = new Config();


            influxDb = config.GetString("InfluxDb", "Db");
            influxDbPort = config.GetInt("InfluxDb", "Port");
            influxDbHost = config.GetString("InfluxDb", "Host");

        }

        public void StoreInverterReadings(List<InverterReadings> inverterReadings)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (InverterReadings r in inverterReadings)
                {

                    sb.Append("inverter");
                    sb.Append($",serialNumber={EscapeString(r.serialNumber)}");
                    sb.Append($",devType={EscapeString(r.devType.ToString())}");

                    sb.Append($" lastReportWatts={r.lastReportWatts}i");

                    sb.Append($" {r.lastReportDate}000000000\n");

                }
                Insert(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "InfluxDb", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }

        }

        public void StoreMeterReadings(List<MeterReading> meterReadings)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (MeterReading m in meterReadings)
                {
                    String eid = EscapeString(m.eid.ToString());

                    sb.Append("meter");
                    sb.Append($",eid={eid}");

                    sb.Append($" actEnergyDlvd={m.actEnergyDlvd}");
                    sb.Append($",actEnergyRcvd={m.actEnergyRcvd}");
                    sb.Append($",apparentEnergy={m.apparentEnergy}");
                    sb.Append($",reactEnergyLagg={m.reactEnergyLagg}");
                    sb.Append($",reactEnergyLead={m.reactEnergyLead}");
                    sb.Append($",instantaneousDemand={m.instantaneousDemand}");
                    sb.Append($",activePower={m.activePower}");
                    sb.Append($",apparentPower={m.apparentPower}");
                    sb.Append($",reactivePower={m.reactivePower}");
                    sb.Append($",pwrFactor={m.pwrFactor}");
                    sb.Append($",voltage={m.voltage}");
                    sb.Append($",current={m.current}");
                    sb.Append($",freq={m.freq}");

                    double totalActivePower = 0;

                    foreach (var c in m.channels)
                    {
                        totalActivePower += c.activePower;
                    }

                    sb.Append($",totalActivePower={totalActivePower}");


                    sb.Append($" {m.timestamp}000000000\n");

                    foreach (var c in m.channels)
                    {
                        sb.Append("meterChannel");
                        sb.Append($",u_eid={eid}");
                        sb.Append($",eid={c.eid}");

                        sb.Append($" actEnergyDlvd={c.actEnergyDlvd}");
                        sb.Append($",actEnergyRcvd={c.actEnergyRcvd}");
                        sb.Append($",apparentEnergy={c.apparentEnergy}");
                        sb.Append($",reactEnergyLagg={c.reactEnergyLagg}");
                        sb.Append($",reactEnergyLead={c.reactEnergyLead}");
                        sb.Append($",instantaneousDemand={c.instantaneousDemand}");
                        sb.Append($",activePower={c.activePower}");
                        sb.Append($",apparentPower={c.apparentPower}");
                        sb.Append($",reactivePower={c.reactivePower}");
                        sb.Append($",pwrFactor={c.pwrFactor}");
                        sb.Append($",voltage={c.voltage}");
                        sb.Append($",current={c.current}");
                        sb.Append($",freq={c.freq}");

                        sb.Append($" {c.timestamp}000000000\n");
                    }

                }
                Insert(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "InfluxDb", "Exception: " + ex.Message + "\n" + ex.StackTrace);
            }

 

        }



        public String Insert(string dbStmt)
        {
            String url = $"http://{influxDbHost}:{influxDbPort}/write?db={influxDb}";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/plain";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(dbStmt);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();

                    if (! responseText.Equals(""))
                        logger.Log(Logger.Level.Error, "InfluxDb", responseText);

                    return responseText;
                }
            }
            catch (WebException ex)
            {
                String resp = "Exception: ";
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        resp += reader.ReadToEnd();
                        if (resp != null && ! resp.Equals(""))
                            logger.Log(Logger.Level.Error, "InfluxDb", resp);
                        return resp;
                    }
                }
                else
                    resp += ex.Message;
                logger.Log(Logger.Level.Error, "InfluxDb", resp);
                return resp;
            }
            catch (Exception ex)
            {
                logger.Log(Logger.Level.Error, "InfluxDb", "Exception: " + ex.Message + "\n" + ex.StackTrace);
                return ex.Message;
            }
        }


        public String EscapeString(String str)
        {
            str = str.Trim();
            if (str.Trim().Equals("")) str = " ";
            return str.Replace(" ", @"\ ").Replace(",", @"\,").Replace("=", @"\=");
        }
    }
}

