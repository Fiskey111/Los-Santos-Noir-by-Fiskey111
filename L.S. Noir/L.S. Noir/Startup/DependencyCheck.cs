//using System;
//using Rage;

//namespace LSNoir
//{
//    class DependencyCheck
//    {
//        internal static bool BetterEMS()
//        {
//            //TODO: get xml from the internet with the current versions of mods

//            if (PluginCheck.IsLspdfrPluginRunning("BetterEMS", new Version("3.0.6232.42981")))
//            {
//                "BetterEMS Found".AddLog();
//                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir" + "Created by Fiskey111, LtFlash, Albo1125", "L.S. Noir dependency loaded ~g~successfully~w~", "BetterEMS ~g~found~w~\n\nEnjoy!");
//                return true;
//            }
//            else if (PluginCheck.IsLspdfrPluginRunning("BetterEMS"))
//            {
//                "BetterEMS out-of-date".AddLog();
//                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir", "Created by Fiskey111, LtFlash, Albo1125", "L.S. Noir dependency loaded ~r~unsuccessfully~w~\nBetterEMS version ~r~not supported~w~\nUpdate BetterEMS to have a better experience!\n\nEnjoy!");
//                return false;
//            }
//            else
//            {
//                "BetterEMS not found".AddLog();
//                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir", "Created by Fiskey111, LtFlash, Albo1125", "L.S. Noir dependency loaded ~r~unsuccessfully~w~\nBetterEMS ~r~not found~w~\nInstall BetterEMS to have a better experience!\n\nEnjoy!");
//                return false;
//            }
//        }
//    }
//}
