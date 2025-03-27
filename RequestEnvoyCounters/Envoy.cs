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
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Net.Sockets;
using System.Text.Json;

namespace RequestEnvoyCounters
{
    internal class Envoy
    {
        InfluxDb influxDb = new InfluxDb();

        Logger log = new Logger("Envoy");

        private static String url = "";

        private string access_token = "eyJhbGciOiJSUzI1NiJ9.eyJhcHBfdHlwZSI6InN5c3RlbSIsInVzZXJfbmFtZSI6ImZtY251bmVzQGdtYWlsLmNvbSIsImVubF9jaWQiOiIiLCJlbmxfcGFzc3dvcmRfbGFzdF9jaGFuZ2VkIjoiMTc0MzA2OTA3OSIsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJjbGllbnRfaWQiOiIyZWE3NzA4NmQ3ZTFjMTlhZTY4MDQ0MjI1ODRlODllZCIsImF1ZCI6WyJvYXV0aDItcmVzb3VyY2UiXSwiaXNfaW50ZXJuYWxfYXBwIjpmYWxzZSwic2NvcGUiOlsicmVhZCIsIndyaXRlIl0sImV4cCI6MTc0MzE4ODg3NiwiZW5sX3VpZCI6IjMyNjU1ODciLCJhcHBfSWQiOiIxNDA5NjI1NTYyNjUxIiwianRpIjoiMjEyYWJhZWItYjViOC00NjA5LWFkNTYtMDFjZjkxZTA2ZWVlIn0.D8AhV2-tiXWl24JTvjxb4Kx_d0qQsMEsv_eTZAVlmoLEYOizlSsnVXGAh7or4vLNAF6tYzxiScEZepS8w20qBVvW7ckpK4ul9bqWQmATDH8HqRiFdnBhc1jdu_G8ewFa_gEWL7hhaYdPP1BbOFVWM1p_2xVbK1YguUykQXMsHx8";
        private string refresh_token = "eyJhbGciOiJSUzI1NiJ9.eyJhcHBfdHlwZSI6InN5c3RlbSIsInVzZXJfbmFtZSI6ImZtY251bmVzQGdtYWlsLmNvbSIsImVubF9jaWQiOiIiLCJlbmxfcGFzc3dvcmRfbGFzdF9jaGFuZ2VkIjoiMTc0MzA2OTA3OSIsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJjbGllbnRfaWQiOiIyZWE3NzA4NmQ3ZTFjMTlhZTY4MDQ0MjI1ODRlODllZCIsImF1ZCI6WyJvYXV0aDItcmVzb3VyY2UiXSwiaXNfaW50ZXJuYWxfYXBwIjpmYWxzZSwic2NvcGUiOlsicmVhZCIsIndyaXRlIl0sImF0aSI6IjIxMmFiYWViLWI1YjgtNDYwOS1hZDU2LTAxY2Y5MWUwNmVlZSIsImV4cCI6MTc0NTczMjIyMiwiZW5sX3VpZCI6IjMyNjU1ODciLCJhcHBfSWQiOiIxNDA5NjI1NTYyNjUxIiwianRpIjoiNzIxYTI2MTctOThjMy00ZjM5LTkyOTYtMGY1NTQzMDU1YzMxIn0.FseNk6XAkFHQnLTyxEN3RN0amqEig4Lpa-3Ipyk3EwDz-VS7nw8RuxLZJ6ktHag0rvtLQsve76qSUPzBViuPgrHjPBTdf_oK0bA1iD_ilHH9pUh3sZlT6XLpR-ty_7N_xN5S7LXgVtrrYPl_EYRWCScf1qeVy2vkDbnNRBSqO54";


        public Envoy()
        {
            Config config = new Config();

            url =      config.GetString("Envoy", "url");
			
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        }

        public void GetToken()
        {

            string clientId = "2ea77086d7e1c19ae6804422584e89ed";
            string clientSecret = "e727f22c26ebe45c6e018e9378ca77ea";
            string redirectUri = "/";
 
            string code = "3PJwfk";
            
            string url = $"https://api.enphaseenergy.com/oauth/token?grant_type=authorization_code&redirect_uri={redirectUri}&code={code}";

            using (HttpClient client = new HttpClient())
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpContent emptyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(url, emptyContent).Result; // Synchronous POST
                string responseString = response.Content.ReadAsStringAsync().Result; // Synchronous read

                var jsonDoc = JsonDocument.Parse(responseString);
                string accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();
                string refreshToken = jsonDoc.RootElement.GetProperty("refresh_token").GetString();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"Refresh Token: {refreshToken}");

                Console.WriteLine(responseString);
            }

        }

        public void RefreshToken()
        {

            string clientId = "2ea77086d7e1c19ae6804422584e89ed";
            string clientSecret = "e727f22c26ebe45c6e018e9378ca77ea";


            string code = "3PJwfk";

            string url = $"https://api.enphaseenergy.com/oauth/token?grant_type=refresh_token&refresh_token={refresh_token}";

            using (HttpClient client = new HttpClient())
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpContent emptyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(url, emptyContent).Result; // Synchronous POST
                string responseString = response.Content.ReadAsStringAsync().Result; // Synchronous read

                var jsonDoc = JsonDocument.Parse(responseString);
                access_token = jsonDoc.RootElement.GetProperty("access_token").GetString();
                string refreshToken = jsonDoc.RootElement.GetProperty("refresh_token").GetString();

                Console.WriteLine($"Access Token: {access_token}");
                Console.WriteLine($"Refresh Token: {refreshToken}");

                Console.WriteLine(responseString);
            }

        }

        public void Test()
        {
            string system_id = "3690442";
            string key = "026b8890a2d3dd672a09dd7d5fb761af";

            RefreshToken();

            string url = $"https://api.enphaseenergy.com/api/v4/systems/{system_id}/production_meter_readings?key={key}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                HttpContent emptyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.GetAsync(url).Result; // Synchronous POST
                string responseString = response.Content.ReadAsStringAsync().Result; // Synchronous read

                var jsonDoc = JsonDocument.Parse(responseString);
                string accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();
                string refreshToken = jsonDoc.RootElement.GetProperty("refresh_token").GetString();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"Refresh Token: {refreshToken}");

                Console.WriteLine(responseString);
            }

        }

        public void meter_readings()
        {
            string system_id = "3690442";
            string key = "026b8890a2d3dd672a09dd7d5fb761af";

            string url = $"https://api.enphaseenergy.com/api/v4/systems/{system_id}/production_meter_readings?key={key}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                HttpContent emptyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.GetAsync(url).Result; // Synchronous POST
                string responseString = response.Content.ReadAsStringAsync().Result; // Synchronous read

                var jsonDoc = JsonDocument.Parse(responseString);
                string accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();
                string refreshToken = jsonDoc.RootElement.GetProperty("refresh_token").GetString();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"Refresh Token: {refreshToken}");

                Console.WriteLine(responseString);
            }

        }

        /*
         
         {"system_id":3690442,"meter_readings":[{"serial_number":"122131001362EIM1","value":7758815,"read_at":1743102900}],"meta":{"status":"micro","last_report_at":1743103489,"last_energy_at":1743102901,"operational_at":1668874438}}
         */

        /*
 {
  "access_token" : "eyJhbGciOiJSUzI1NiJ9.eyJhcHBfdHlwZSI6InN5c3RlbSIsInVzZXJfbmFtZSI6ImZtY251bmVzQGdtYWlsLmNvbSIsImVubF9jaWQiOiIiLCJlbmxfcGFzc3dvcmRfbGFzdF9jaGFuZ2VkIjoiMTc0MzA2OTA3OSIsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJjbGllbnRfaWQiOiIyZWE3NzA4NmQ3ZTFjMTlhZTY4MDQ0MjI1ODRlODllZCIsImF1ZCI6WyJvYXV0aDItcmVzb3VyY2UiXSwiaXNfaW50ZXJuYWxfYXBwIjpmYWxzZSwic2NvcGUiOlsicmVhZCIsIndyaXRlIl0sImV4cCI6MTc0MzE4ODg3NiwiZW5sX3VpZCI6IjMyNjU1ODciLCJhcHBfSWQiOiIxNDA5NjI1NTYyNjUxIiwianRpIjoiMjEyYWJhZWItYjViOC00NjA5LWFkNTYtMDFjZjkxZTA2ZWVlIn0.D8AhV2-tiXWl24JTvjxb4Kx_d0qQsMEsv_eTZAVlmoLEYOizlSsnVXGAh7or4vLNAF6tYzxiScEZepS8w20qBVvW7ckpK4ul9bqWQmATDH8HqRiFdnBhc1jdu_G8ewFa_gEWL7hhaYdPP1BbOFVWM1p_2xVbK1YguUykQXMsHx8",
  "token_type" : "bearer",
  "refresh_token" : "eyJhbGciOiJSUzI1NiJ9.eyJhcHBfdHlwZSI6InN5c3RlbSIsInVzZXJfbmFtZSI6ImZtY251bmVzQGdtYWlsLmNvbSIsImVubF9jaWQiOiIiLCJlbmxfcGFzc3dvcmRfbGFzdF9jaGFuZ2VkIjoiMTc0MzA2OTA3OSIsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJjbGllbnRfaWQiOiIyZWE3NzA4NmQ3ZTFjMTlhZTY4MDQ0MjI1ODRlODllZCIsImF1ZCI6WyJvYXV0aDItcmVzb3VyY2UiXSwiaXNfaW50ZXJuYWxfYXBwIjpmYWxzZSwic2NvcGUiOlsicmVhZCIsIndyaXRlIl0sImF0aSI6IjIxMmFiYWViLWI1YjgtNDYwOS1hZDU2LTAxY2Y5MWUwNmVlZSIsImV4cCI6MTc0NTczMjIyMiwiZW5sX3VpZCI6IjMyNjU1ODciLCJhcHBfSWQiOiIxNDA5NjI1NTYyNjUxIiwianRpIjoiNzIxYTI2MTctOThjMy00ZjM5LTkyOTYtMGY1NTQzMDU1YzMxIn0.FseNk6XAkFHQnLTyxEN3RN0amqEig4Lpa-3Ipyk3EwDz-VS7nw8RuxLZJ6ktHag0rvtLQsve76qSUPzBViuPgrHjPBTdf_oK0bA1iD_ilHH9pUh3sZlT6XLpR-ty_7N_xN5S7LXgVtrrYPl_EYRWCScf1qeVy2vkDbnNRBSqO54",
  "expires_in" : 86399,
  "scope" : "read write",
  "enl_uid" : "3265587",
  "enl_cid" : "",
  "enl_password_last_changed" : "1743069079",
  "is_internal_app" : false,
  "app_type" : "system",
  "app_Id" : "1409625562651",
  "jti" : "212abaeb-b5b8-4609-ad56-01cf91e06eee"
}
        */

        public void Run()
        {
            Thread t = new Thread(new ThreadStart(LoopInverters));
            t.Start();
            //t = new Thread(new ThreadStart(LoopMeeters));
            //t.Start();
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

                    ReadInvertersX();

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

        private void ReadInvertersX()
        {
            String responseString = RequestData($"{url}/api/v4/systems/3690442/production_meter_readings");

            List<InverterReadings> inverterReadings = JsonConvert.DeserializeObject<List<InverterReadings>>(responseString);

            log.Log(Logger.Level.Info, "ReadInverters", $"Received {inverterReadings.Count} metrics.");

            influxDb.StoreInverterReadings(inverterReadings);
        }

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

            request.Headers.Add("Authorization", $"Bearer {access_token}");
            log.Log(Logger.Level.Info, "ReadMeters", $"Bearer {access_token}");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
    }
}
