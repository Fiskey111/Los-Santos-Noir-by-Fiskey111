//using Rage;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using Fiskey111Common;

//namespace LSNoir.Callouts.SA.Commons
//{
//    class ApiWrapper
//    {
//        public static void RequestEms(Vector3 loc, Queue<Vector3> queue)
//        {
//            if (PluginCheck.IsLspdfrPluginRunning("BetterEMS", new Version("3.0.6232.42981")) == true)
//            {
//                EmsRespond(loc, queue);
//            }
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        private static void EmsRespond(Vector3 location, Queue<Vector3> queue)
//        {
//            BetterEMS.API.EMSFunctions.RespondToLocation(location, false, "EMS requested by first officer", queue);
//        }

//        public static void SetVictimData(Ped ped, string injury, string cause, float survivability)
//        {
//            if (PluginCheck.IsLspdfrPluginRunning("BetterEMS", new Version("2.0.6056.26204")) == true)
//            {
//                VictimData(ped, injury, cause, survivability);
//            }
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        private static void VictimData(Ped ped, string injury, string cause, float survivability)
//        {
//            int l = Rand.RandomNumber(0100, 2400);
//            BetterEMS.API.EMSFunctions.OverridePedDeathDetails(ped, injury, cause, Convert.ToUInt32(l), survivability);
//        }


//        public static bool? WasPedRevived(Ped ped)
//        {
//            if (PluginCheck.IsLspdfrPluginRunning("BetterEMS", new Version("2.0.6056.26204")) == true)
//            {
//                return EmsRevivePed(ped);
//            }
//            else
//            {
//                return null;
//            }
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        private static bool? EmsRevivePed(Ped ped)
//        {
//            return BetterEMS.API.EMSFunctions.DidEMSRevivePed(ped);
//        }
//    }
//}
