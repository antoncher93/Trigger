using System;
using System.Collections.Generic;
using Trigger.Beacons;
using Trigger.Interfaces;
using Trigger.Rangers;
using System.Data.SqlClient;
using Trigger.Signal;
using Trigger.Classes;
using Trigger;

namespace RangerTest
{
    class Program
    {
        const string SqlConnectionString = @"Data Source=192.168.0.9;Initial Catalog=Shoppercoin;Persist Security Info=True;User ID=ivg;Password=ivg";

        static void Main(string[] args)
        {
            IRanger ranger = new TimeRangerBuilder()
               .AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("ebefd083-70a2-47c8-9837-e7b5634df525")))
               .AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("ebefd083-70a2-47c8-9837-e7b5634df599")))
               .Build();

            ranger.OnEvent += (s, e) =>
            {
                Console.WriteLine(e.Type + " " + e.Timespan);

                switch (e.Type)
                {
                    case Trigger.Enums.TriggerEventType.Enter:

                        break;
                    case Trigger.Enums.TriggerEventType.Exit:
                        break;
                }
            };

            Console.WriteLine("Hello World!");

            var con = ConnectToDB();
            var strs = GetStringsFromSqlConnection(con);

            foreach(var s in strs)
            {
                var telemetry = Newtonsoft.Json.JsonConvert.DeserializeObject<Telemetry>(s, new TelemetryJsonConverter());

                Console.WriteLine(telemetry.Protocol);

                ranger.OnNext(telemetry);
            }



            Console.ReadKey();
        }

        static SqlConnection ConnectToDB()
        {
            var result = new List<string>();

            SqlConnection conn = new SqlConnection(SqlConnectionString);

            conn.Open();

            return conn;
        }

        static ICollection<string> GetStringsFromSqlConnection(SqlConnection openedCon)
        {
            var result = new List<string>();
            if(openedCon?.State == System.Data.ConnectionState.Open)
            {
                var reader = new SqlCommand("select Data from signal", openedCon).ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }
    }
}
