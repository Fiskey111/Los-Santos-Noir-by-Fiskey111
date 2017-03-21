using System.Collections.Generic;

namespace LSNoir.Callouts.SA.Data
{
    public class WitnessData
    {
        public static Dictionary<int, string> WhatSeen()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "Blue Futo with 76 on the plate");
            dict.Add(1, "Yellow Cavalcade with E7Y on the plate");
            dict.Add(2, "Caucasian Male");
            dict.Add(3, "Caucasian Female");
            dict.Add(4, "Black Male");
            dict.Add(5, "Black Female");
            dict.Add(6, "Two people fighting over something");
            dict.Add(7, "A person crying in the alleyway laying down");
            return dict;
        }
    }
}
