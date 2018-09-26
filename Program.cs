﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigger.Telemetry.Beacons;
using Trigger.Telemetry;
using System.IO;
using Trigger;

namespace TriggerTest
{
    class Program
    {
        static void Main(string[] args)
        {

            // мак адреса биконов первой, второй и вспомогательной линий
            string FirstBeacon = "DF:20:C6:5A:62:5F";
            string SecondBeacon = "E3:25:3E:0A:7E:4C";
            string HelpBeacon = "DE:A6:78:08:52:A2";
            //E3:25:3E:0A:7E:4C
            //DE:A6:78:08:52:A2
            string path = @"D:\Telemetry\telemetry.txt";

            // строка полученной телеметрии
            string telemetry = File.ReadAllText(path);
            string telemetry2 = File.ReadAllText(@"D:\Telemetry\telemetry2.txt");
           
            
            // десереализуем string в объект
            int c = telemetry.Length;
            Telemetry ob = Newtonsoft.Json.JsonConvert.DeserializeObject<Telemetry>(telemetry);

            // build telemetry ranger
            Ranger ranger = new RangerBuilder()
                .SetCalcSlideAverageCount(4) // коэф скольжения для настройки фильтрации
                .AddFirstLineBeacon(BeaconBody.Parse(FirstBeacon)) // бикон первой линии
                .AddSecondLineBeacon(BeaconBody.Parse(SecondBeacon)) // бикон второй линии
                .AddHelpBeacon( BeaconBody.Parse(HelpBeacon)) // вспомогательный бикон
                .SetAPointUid(null) // задаем Uid пользователя
                .Build();

            ranger.Enter += (s, e) => Console.WriteLine("Enter at " + e.DateTime);// подписка на событие входа
            ranger.Exit += (s, e) => Console.WriteLine("Exit at " + e.DateTime);// подписка на выход

            ranger.CheckTelemetry(ob); // проверяем ранжировщиком телеметрию

            Console.ReadKey();
        }
    }
}
