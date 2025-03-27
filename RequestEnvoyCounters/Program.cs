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
    internal class Program
    {
        InfluxDb influxDb = new InfluxDb();

        Logger log = new Logger("main");

        static void Main(string[] args)
        {
            Envoy envoy = new Envoy();

            envoy.Test();
        }
    }
}
