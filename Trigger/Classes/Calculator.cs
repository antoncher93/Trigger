using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Classes
{
    public class Calculator
    {
        private const double GtGr = 0.25;

        public static double DbmToMicroW(double dbm)
        {
            return Math.Pow(10, dbm / 10);
        }

        public static double BeaconDiastance(double beaconRssi, double transPower)
        {
            double result = 0;
            double beaconRssi_mW = DbmToMicroW( beaconRssi);
            double beaconTPow_mW = DbmToMicroW(transPower);
            double c = Math.Pow(300.0 / 2445.0, 2.0);
            double a = beaconTPow_mW * GtGr * c;
            double b = Math.Pow(4 * Math.PI, 2) * beaconRssi_mW;
            result = Math.Sqrt(a / b) ;

            return result;
        }

        public double CommonChord(double r1, double r2, double d)
        {
            return 0;
        }
    }//
}
