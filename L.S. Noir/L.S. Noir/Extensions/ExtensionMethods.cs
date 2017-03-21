using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LSNoir.Extensions;

namespace LSNoir.Callouts.SA.Commons
{
    public static class ExtensionMethods
    {
        public static void MakeMissionPed(this Ped ped)
        {
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = true;
        }

        public static void MakeMissionVehicle(this Vehicle veh)
        {
            veh.IsPersistent = true;
        }

        public static void MakeMissionObject(this Rage.Object obj)
        {
            obj.IsPersistent = true;
        }
        
        public static void LogDistanceFromCallout(Vector3 calloutLocation)
        {
            float distance = Vector3.Distance(calloutLocation, Game.LocalPlayer.Character.Position);
            ("Player is: " + distance.ToString() + "f away from the location").AddLog();
        }

        public static void Task_Scenario(this Ped ped, string scenario)
        {
            NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(ped, scenario.ToString(), 0, true);
        }

        public static void Task_Scenario(this Ped ped, string scenario, Vector3 position, float heading)
        {
            NativeFunction.Natives.TASK_START_SCENARIO_AT_POSITION(ped, scenario, position.X, position.Y, position.Z, heading, 0, 0, 1);
        }

        /*public static void StartCalloutCheck()
        {
            _calloutFiber = new GameFiber(CalloutFiber);
            _calloutFiber.Start();
        }

        public static void StopCalloutCheck()
        {
            _calloutFiber.Abort();
        }

        private static void CalloutFiber()
        {
            while (true)
            {
                if (Functions.IsCalloutRunning() == true)
                {
                    ExtensionMethods.AddLog("Stopping Callout...");
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Dispatch:", "Redirecting Callout", "Callout has been redirected to another unit, continue working on case");
                    Functions.StopCurrentCallout();
                }
                GameFiber.Yield();
            }
        }*/

        public static bool IsPolicePed(this Ped ped)
        {
            return ped.RelationshipGroup == "COP";
        }

        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }
        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static string GetKeyString(this Keys mainKey, Keys modifierKey)
        {
            if (modifierKey == Keys.None)
            {
                return mainKey.ToString();
            }
            else
            {
                string strmodKey = modifierKey.ToString();

                if (strmodKey.EndsWith("ControlKey") | strmodKey.EndsWith("ShiftKey"))
                {
                    strmodKey.Replace("Key", "");
                }

                if (strmodKey.Contains("ControlKey"))
                {
                    strmodKey = "CTRL";
                }
                else if (strmodKey.Contains("ShiftKey"))
                {
                    strmodKey = "Shift";
                }
                else if (strmodKey.Contains("Menu"))
                {
                    strmodKey = "ALT";
                }

                return string.Format("{0} + {1}", strmodKey, mainKey.ToString());
            }
        }

        public static Gender GetGender(Ped p)
        {
            if (p.IsMale)
                return Gender.Male;
            else
                return Gender.Female;
        }

        public enum Gender { Male, Female }
    }

    internal static class KeysMethods
    {
        public static bool IsKeyDownComputerCheck(Keys keyPressed)
        {
            if (NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD") != 0)
            {

                return Game.IsKeyDown(keyPressed);
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyDownRightNowComputerCheck(Keys keyPressed)
        {
            if (NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD") != 0)
            {
                return Game.IsKeyDownRightNow(keyPressed);
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyCombinationDownComputerCheck(Keys mainKey, Keys modifierKey)
        {
            if (mainKey != Keys.None)
            {
                return IsKeyDownComputerCheck(mainKey) && (IsKeyDownRightNowComputerCheck(modifierKey)
                || (modifierKey == Keys.None && !IsKeyDownRightNowComputerCheck(Keys.Shift) && !IsKeyDownRightNowComputerCheck(Keys.Control)
                && !IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !IsKeyDownRightNowComputerCheck(Keys.LShiftKey)));
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyCombinationDownRightNowComputerCheck(Keys mainKey, Keys modifierKey)
        {
            if (mainKey != Keys.None)
            {
                return IsKeyDownRightNowComputerCheck(mainKey) && ((IsKeyDownRightNowComputerCheck(modifierKey)
                    || (modifierKey == Keys.None && !IsKeyDownRightNowComputerCheck(Keys.Shift) && !IsKeyDownRightNowComputerCheck(Keys.Control)
                    && !IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !IsKeyDownRightNowComputerCheck(Keys.LShiftKey))));
            }
            else
            {
                return false;
            }

        }
    }
}
