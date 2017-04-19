using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Evidence;
using Rage;
using Rage.Native;
using StageObjects;
using Animations = AnimationDatabase.Animations;
using SpawnPoint = LtFlash.Common.SpawnPoint;

namespace LSNoir.Callouts.SA.Stages.CSI
{
    class CsiCreator
    {
        internal static DeadBody Victim;
        internal static float FirstOfficerHeading;
        internal static StageObject FirstOfficer = new StageObject();
        internal static StageObject Officer1 = new StageObject();
        internal static StageObject Officer2 = new StageObject();
        internal static StageObject Officer3 = new StageObject();
        internal static StageObject Officer4 = new StageObject();
        internal static StageObject Fbi1 = new StageObject();
        internal static StageObject Fbi2 = new StageObject();
        internal static StageObject PoliceCar1 = new StageObject();
        internal static StageObject PoliceCar2 = new StageObject();
        internal static StageObject PoliceCar3 = new StageObject();
        internal static StageObject Ped1 = new StageObject();
        internal static StageObject Ped2 = new StageObject();
        internal static StageObject Ped3 = new StageObject();
        internal static StageObject Ped4 = new StageObject();
        internal static StageObject Ped5 = new StageObject();
        internal static StageObject Barrier1 = new StageObject();
        internal static StageObject Barrier2 = new StageObject();
        internal static StageObject Barrier3 = new StageObject();
        internal static StageObject Barrier4 = new StageObject();
        internal static StageObject Barrier5 = new StageObject();

        internal static bool IsCompleted = false;

        internal static Animations Clip1, Clip2, Idles1, Idles2, MmobileFilm, FmobileFilm, Pap, Sec, Guard1, Guard2, MobileCall, MobileText1;
        internal static List<Animations> CanimationList, PanimationList;
        internal static Queue<SpawnPoint> EmsQueue;
        internal static SpawnPoint EmsLast;

        internal static string[] Vmodel = { "a_f_y_topless_01", "csb_stripper_02", "a_f_m_beach_01", "s_f_y_stripper_01", "a_m_m_beach_01" };
        internal static List<string> Pcarmodel = Settings.VehicleList();
        internal static string[] Fbimodel = { "s_m_m_fiboffice_01", "s_m_m_fiboffice_02" };
        internal static string[] Copmodel = { "s_m_y_cop_01", "s_f_y_cop_01" };
        internal static string[] Pedmodel = { "a_m_m_genfat_01", "a_m_m_genfat_02", "a_f_y_genhot_01", "a_m_o_genstreet_01", "a_m_y_genstreet_01", "a_m_y_genstreet_02" };

        internal static List<StageObject> ObjectList = new List<StageObject>();

        internal static XDocument Xml;
        internal static Stopwatch Sw = new Stopwatch();

        internal static GameFiber CopFiber, AmbientFiber;
        internal static int OfficerNum, WitnessNum, VehicleNum, FbiNum, PedNum;
        internal static string Root;

        internal const string Xmlpath = @"Plugins\LSPDFR\LSNoir\SA\Data\SceneData.xml";

        internal static ReportData FOdata;
        internal static PedData Wit1Data, Wit2Data;

        internal static Dialog FoDialog, Wit1Dialog, Wit2Dialog;

        internal static bool Completed, Witness1Exists = false, Witness2Exists = false, CompletelyFinished = false, loadCreated = false;
        internal static bool MComplete = false, AComplete, CComplete;

        internal static SpawnPoint SecCamSpawnPoint, SusSpawnPoint, SusTargetPoint, _W1SpawnPoint, _W2SpawnPoint;

        /*
        Animations:       
                _firstOfficer.Ped.Tasks.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", 4, AnimationFlags.Loop);
        */
        /// <summary>
        /// Call this to begin scene creation using SceneData.xml
        /// </summary>
        /// <param name="playerPosition"></param>
        public static void CreateScene(Vector3 playerPosition)
        {
            "Starting Scene Creation -- Stopwatch Started".AddLog();
            Sw.Start();
            ObjectList.Clear();
            LoadXml();
            "XML Loaded".AddLog();
            var playerdist = Vector3.Distance(playerPosition, Victim.Position);
            ("Distance from Scene: " + playerdist).AddLog();
            SetAnimations();
            AmbientFiber = new GameFiber(AmbientRun);
            AmbientFiber.Start();
            CopFiber = new GameFiber(CopRun);
            CopFiber.Start();
            Completed = true;
        }

        private static void LoadXml()
        {
            try
            {
                "Retreiving XML Data".AddLog();
                Xml = XDocument.Load(Xmlpath);
                ("XML Loaded from " + Xmlpath).AddLog();

                var count = XDocument.Load(Xmlpath).XPathSelectElements("CSI/Location").Count();
                ("Number of Locations: " + count.ToString()).AddLog();

                var l = Rand.RandomNumber(6);
                switch (l)
                {
                    case 0:
                        Root = "CSI/Location/Storage";
                        break;
                    case 1:
                        Root = "CSI/Location/Alta";
                        break;
                    case 2:
                        Root = "CSI/Location/Greenwich";
                        break;
                    case 3:
                        Root = "CSI/Location/Grove";
                        break;
                    case 4:
                        Root = "CSI/Location/LaborPl";
                        break;
                    case 5:
                        Root = "CSI/Location/NorthRockfordDr";
                        break;
                }
                $"Scene: {Root}".AddLog(true);
                var sp = GetSpawn("/Victim/");
                Victim = new DeadBody("Vic", "Victim", sp, Vmodel[Rand.RandomNumber(Vmodel.Length)]);
                "Victim created".AddLog();

                OfficerNum = Rand.RandomNumber(1, 5);
                VehicleNum = Rand.RandomNumber(0, 4);
                FbiNum = Rand.RandomNumber(0, 3);
                PedNum = Rand.RandomNumber(0, 6);

                ("Victim Model: " + Victim.Ped.Model.Name).AddLog();

                if (Xml.Root.XPathSelectElement(Root + "/Officers/FirstOfficer/x").Value != "null")
                {
                    var spawn = GetSpawn("/Officers/FirstOfficer/");

                    FirstOfficer = new StageObject("s_m_y_cop_01", spawn.Position, spawn.Heading);

                    FirstOfficer.Ped.MakeMissionPed();
                    Functions.SetCopAsBusy(FirstOfficer.Ped, true);

                    FirstOfficer.Exists = true;

                    ObjectList.Add(FirstOfficer);
                }
            ("FirstOfficer.Exists = " + FirstOfficer.Exists).AddLog();

                CreateStage_Object(Officer1, "/Officers/Officer1/", ObjectType.Cop);

                CreateStage_Object(Officer2, "/Officers/Officer2/", ObjectType.Cop);

                CreateStage_Object(Officer3, "/Officers/Officer3/", ObjectType.Cop);

                CreateStage_Object(Officer4, "/Officers/Officer4/", ObjectType.Cop);

                _W1SpawnPoint = new SpawnPoint(GetSpawn("/Witnesses/Witness1/").Heading,
                    GetSpawn("/Witnesses/Witness1/").Position);

                _W2SpawnPoint = new SpawnPoint(GetSpawn("/Witnesses/Witness2/").Heading,
                    GetSpawn("/Witnesses/Witness2/").Position);

                CreateStage_Object(PoliceCar1, "/Vehicles/Vehicle1/", ObjectType.Vehicle);

                CreateStage_Object(PoliceCar2, "/Vehicles/Vehicle2/", ObjectType.Vehicle);

                CreateStage_Object(PoliceCar3, "/Vehicles/Vehicle3/", ObjectType.Vehicle);

                CreateStage_Object(Fbi1, "/Ambient/FBI1/", ObjectType.Cop);

                CreateStage_Object(Fbi2, "/Ambient/FBI2/", ObjectType.Cop);

                CreateStage_Object(Ped1, "/Ambient/Ped1/", ObjectType.Ped);

                CreateStage_Object(Ped2, "/Ambient/Ped2/", ObjectType.Ped);

                CreateStage_Object(Ped3, "/Ambient/Ped3/", ObjectType.Ped);

                CreateStage_Object(Ped4, "/Ambient/Ped4/", ObjectType.Ped);

                CreateStage_Object(Ped5, "/Ambient/Ped5/", ObjectType.Ped);

                EmsQueue = new Queue<SpawnPoint>();
                EmsQueue.Enqueue(GetSpawn("/EMS/Queue/"));

                EmsLast = GetSpawn("/EMS/FinalPos/");

                SecCamSpawnPoint = GetSpawn("/Camera/");
                SusSpawnPoint = GetSpawn("/SusSpawn/");
                SusTargetPoint = GetSpawn("/SusTarget/");

                $" ***SPAWNPOINTS*** {SecCamSpawnPoint.Position.X} {SusSpawnPoint.Position.X} {SusTargetPoint.Position.X} ".AddLog(true);

                loadCreated = true;

            }
            catch (Exception e)
            {
                e.ToString().AddLog(true);
            }
        }
        /// <summary>
        /// Call this in order to add barriers to the crime scene
        /// </summary>
        public static void AddBarriers()
        {
            CreateStage_Object(Barrier1, "/Barriers/Barrier1/", ObjectType.Barrier);

            CreateStage_Object(Barrier2, "/Barriers/Barrier2/", ObjectType.Barrier);

            CreateStage_Object(Barrier3, "/Barriers/Barrier3/", ObjectType.Barrier);

            CreateStage_Object(Barrier4, "/Barriers/Barrier4/", ObjectType.Barrier);

            CreateStage_Object(Barrier5, "/Barriers/Barrier5/", ObjectType.Barrier);
        }

        private static void CreateStage_Object(StageObject obj, string location, ObjectType type, string propName = "prop_barrier_work05")
        {
            ("Creating " + type + "  using location " + location).AddLog();
            if (Xml.Root.XPathSelectElement(Root + location + "x").Value != "null")
            {
                obj.Spawn = GetSpawn(location).Position;
                obj.Heading = GetSpawn(location).Heading;
                ("Heading: " + obj.Heading).AddLog();
                switch (type)
                {
                    case ObjectType.Ped:
                    case ObjectType.Cop:
                    case ObjectType.Witness:
                        obj.ObjectType = StageObject.Type.Ped;
                        switch (type)
                        {
                            case ObjectType.Cop:
                                obj.Ped = new Ped(Copmodel[Rand.RandomNumber(1, Copmodel.Length)], obj.Spawn, obj.Heading);
                                Functions.SetCopAsBusy(obj.Ped, true);
                                break;
                            case ObjectType.Witness:
                                new Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness(
                                    "Witness", "Witness", new Services.SpawnPoint(obj.Heading, obj.Spawn),
                                    Model.PedModels.ToList()[MathHelper.GetRandomInteger(Model.PedModels.ToList().Count)],
                                    new Dialog(Wit1Dialog.Lines, obj.Spawn), EmsLast.Position);
                                obj.Ped = new Ped(Pedmodel[Rand.RandomNumber(1, Pedmodel.Length)], obj.Spawn, obj.Heading);

                                var helpful = MathHelper.GetRandomInteger(5) == 1;
                                obj.IsImportant = helpful;
                                break;
                            case ObjectType.Ped:
                                obj.Ped = new Ped(Pedmodel[Rand.RandomNumber(1, Pedmodel.Length)], obj.Spawn, obj.Heading);
                                break;
                        }
                        NativeFunction.Natives.CLEAR_AREA_OF_OBJECTS(obj.Spawn.X, obj.Spawn.Y, obj.Spawn.Z, 0.50f, 0);
                        obj.Ped.IsInvincible = true;
                        obj.Ped.Heading = obj.Heading;
                        obj.Ped.RandomizeVariation();
                        obj.Ped.MakeMissionPed();
                        break;
                    case ObjectType.Vehicle:
                        obj.ObjectType = StageObject.Type.Vehicle;
                        obj.Vehicle =
                            new Vehicle(Settings.VehicleList()[Rand.RandomNumber(Settings.VehicleList().Count)],
                                obj.Spawn, obj.Heading);
                        obj.Vehicle.MakeMissionVehicle();
                        if (Rand.RandomNumber(1, 3) == 1)
                        {
                            obj.Vehicle.IsSirenOn = true;
                            obj.Vehicle.IsSirenSilent = true;
                        }
                        obj.Vehicle.IsEngineOn = true;
                        break;
                    case ObjectType.Barrier:
                        obj.ObjectType = StageObject.Type.Barrier;
                        obj.Object = new Rage.Object(propName, obj.Spawn, obj.Heading) { IsGravityDisabled = false };
                        obj.Object.MakeMissionObject();
                        break;
                }
                obj.Exists = true;
                ObjectList.Add(obj);
                (type + " created successfully").AddLog();
            }
            else
            {
                "Location returns null".AddLog();
            }
        }

        private static SpawnPoint GetSpawn(string location)
        {
            if (Xml.Root.XPathSelectElement(Root + location + "x").Value == "null") return SpawnPoint.Zero;

            var sp = new SpawnPoint
            {
                Position =
                    new Vector3(
                        float.Parse(Xml.Root.XPathSelectElement(Root + location + "x").Value, NumberStyles.Float, CultureInfo.InvariantCulture),
                        float.Parse(Xml.Root.XPathSelectElement(Root + location + "y").Value, NumberStyles.Float, CultureInfo.InvariantCulture),
                        float.Parse(Xml.Root.XPathSelectElement(Root + location + "z").Value, NumberStyles.Float, CultureInfo.InvariantCulture))
            };
            try
            {
                sp.Heading = float.Parse(Xml.Root.XPathSelectElement(Root + location + "h").Value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch
            {
                sp.Heading = 0;
            }
            return sp;
        }

        private static void SetAnimations()
        {
            Clip1 = new Animations("clipboard", "amb@world_human_clipboard@male@idle_a", "idle_c", null, null, null, null, new Model("p_amb_clipboard_01"));
            Clip2 = new Animations("clipboard", "amb@world_human_clipboard@male@idle_b", "idle_d", null, null, null, null, new Model("p_amb_clipboard_01"));
            Sec = new Animations("sec", "amb@world_human_security_shine_torch@male@idle_a", "idle_a", "amb@world_human_security_shine_torch@male@enter", "enter",
                "amb@world_human_security_shine_torch@male@exit", "exit", new Model("WEAPON_FLASHLIGHT"));
            Guard1 = new Animations("guard", "amb@world_human_stand_guard@male@idle_a", "idle_a", "amb@world_human_stand_guard@male@enter", "enter",
                "amb@world_human_stand_guard@male@exit", "exit");
            Guard2 = new Animations("guard", "amb@world_human_stand_guard@male@idle_b", "idle_d", "amb@world_human_stand_guard@male@enter", "enter",
                "amb@world_human_stand_guard@male@exit", "exit");
            Idles1 = new Animations("idle", "amb@world_human_cop_idles@male@idle_a", "idle_b", "amb@world_human_cop_idles@male@idle_enter", "idle_intro", null, null);
            Idles2 = new Animations("idle", "amb@world_human_cop_idles@male@idle_b", "idle_e", "amb@world_human_cop_idles@male@idle_enter", "idle_intro", null, null);
            Pap = new Animations("pap", "amb@world_human_paparazzi@male@idle_a", "idle_a", "amb@world_human_paparazzi@male@enter", "enter",
                "amb@world_human_paparazzi@male@exit", "exit", new Model("prop_pap_camera_01"));
            MmobileFilm = new Animations("mobile", "amb@world_human_mobile_film_shocking@male@idle_a", "idle_c", "amb@world_human_mobile_film_shocking@male@enter", "enter",
                "amb@world_human_mobile_film_shocking@male@exit", "exit", new Model("prop_amb_phone"));
            FmobileFilm = new Animations("mobile", "amb@world_human_mobile_film_shocking@female@idle_a", "idle_a", "amb@world_human_mobile_film_shocking@female@enter", "enter",
                "amb@world_human_mobile_film_shocking@female@exit", "exit", new Model("prop_amb_phone"));
            MobileCall = new Animations("mobile", "amb@world_human_stand_mobile@male@standing@call@idle_a", "idle_a", "amb@world_human_stand_mobile@male@standing@call@enter", "enter",
                "amb@world_human_stand_mobile@male@standing@call@exit", "exit_to_text", "prop_amb_phone");
            MobileText1 = new Animations("mobile", "amb@world_human_stand_mobile@male@text@idle_a", "idle_a", "amb@world_human_stand_mobile@male@text@enter", "enter",
                "amb@world_human_stand_mobile@male@text@exit", "exit_to_call", new Model("prop_amb_phone"));

            CanimationList = new List<Animations> { Clip1, Clip2, Idles1, Idles2, Sec, Guard1, Guard2 };
            PanimationList = new List<Animations>
            {
                MmobileFilm,
                FmobileFilm,
                Pap,
                MobileCall,
                MobileText1
            };
        }

        private static void CopRun()
        {
            StartScenario(Fbi1.Ped, PedType.Cop);

            StartWander(Fbi2.Ped);

            StartScenario(Officer1.Ped, PedType.Cop);

            StartScenario(Officer2.Ped, PedType.Cop);

            StartScenario(Officer3.Ped, PedType.Cop);

            StartWander(Officer4.Ped);

            CComplete = true;
        }

        private static void AmbientRun()
        {
            StartScenario(Ped1.Ped, PedType.Ambient);

            StartScenario(Ped2.Ped, PedType.Ambient);

            StartScenario(Ped3.Ped, PedType.Ambient);

            StartScenario(Ped4.Ped, PedType.Ambient);

            StartScenario(Ped5.Ped, PedType.Ambient);

            AComplete = true;
        }

        private static void StartScenario(Ped ped, PedType type)
        {
            try
            {
                if (ped.Exists())
                {
                    GameFiber.StartNew(delegate
                    {
                        ("Starting scenario for " + type.ToString()).AddLog();
                        string scenario = "";

                        switch (type)
                        {
                            case PedType.Cop:
                                var o1 = CanimationList[MathHelper.GetRandomInteger(CanimationList.Count - 1)];
                                ("Cop animation assigned: " + o1.FirstAnimation).AddLog();

                                switch (o1.Name)
                                {
                                    case "clipboard":
                                        scenario = "WORLD_HUMAN_CLIPBOARD";
                                        break;
                                    case "sec":
                                        scenario = "WORLD_HUMAN_SECURITY_SHINE_TORCH";
                                        break;
                                    case "guard":
                                        scenario = "WORLD_HUMAN_GUARD_STAND";
                                        break;
                                    default:
                                        scenario = "WORLD_HUMAN_COP_IDLES";
                                        break;
                                }
                                break;
                            case PedType.Ambient:
                                var p1 = PanimationList[MathHelper.GetRandomInteger(PanimationList.Count() - 1)];
                                ("Animation assigned: " + p1.FirstAnimation).AddLog();

                                if (p1.Name == "mobile")
                                    scenario = Rand.RandomNumber(1, 3) == 1
                                        ? "WORLD_HUMAN_MOBILE_FILM_SHOCKING"
                                        : "WORLD_HUMAN_STAND_MOBILE";
                                else
                                    scenario = "WORLD_HUMAN_PAPARAZZI";
                                break;
                        }

                        while (true)
                        {
                            if (!ped.Exists() || ped.IsDead) break;
                            ped.Task_Scenario(scenario);
                            while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(ped))
                                GameFiber.Yield();
                            GameFiber.Yield();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ToString().AddLog(true);
            }
        }

        private static void StartWander(Ped ped)
        {
            try
            {
                if (ped.Exists())
                {
                    GameFiber.StartNew(delegate
                    {
                        while (true)
                        {
                            if (!ped.Exists() || ped.IsDead) break;
                            var o1 = CanimationList[MathHelper.GetRandomInteger(CanimationList.Count - 1)];
                            string scenario = "";

                            switch (o1.Name)
                            {
                                case "clipboard":
                                    scenario = "WORLD_HUMAN_CLIPBOARD";
                                    break;
                                case "sec":
                                    scenario = "WORLD_HUMAN_SECURITY_SHINE_TORCH";
                                    break;
                                case "guard":
                                    scenario = "WORLD_HUMAN_GUARD_STAND";
                                    break;
                                default:
                                    scenario = "WORLD_HUMAN_COP_IDLES";
                                    break;
                            }
                            ped.Tasks.Wander();
                            GameFiber.Sleep(MathHelper.GetRandomInteger(5000, 10000));
                            ped.Task_Scenario(scenario);
                            GameFiber.Sleep(0500);
                            while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(ped))
                                GameFiber.Yield();
                            GameFiber.Yield();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ToString().AddLog(true);
            }
        }

        public static void End()
        {
            DeleteObjects(ObjectList);
            "CSI_Creator Ended".AddLog();
        }

        private static void DeleteObjects(List<StageObject> objList)
        {
            if (objList.Count < 1) return;
            var i = 1;
            foreach (var objects in objList)
            {
                switch (objects.ObjectType)
                {
                    case StageObject.Type.Ped:
                        if (objects.Ped) objects.Ped.Delete();
                        ("Deleted object " + i).AddLog();
                        break;
                    case StageObject.Type.Barrier:
                        if (objects.Object.Exists()) objects.Object.Delete();
                        ("Deleted object " + i).AddLog();
                        break;
                    case StageObject.Type.Vehicle:
                        if (objects.Vehicle.Exists()) objects.Vehicle.Delete();
                        ("Deleted object " + i).AddLog();
                        break;
                }
                i++;
            }
            objList.Clear();
            if (CopFiber.IsAlive) CopFiber.Abort();
            if (AmbientFiber.IsAlive) AmbientFiber.Abort();
            if (Victim.Exists()) Victim.Ped.Delete();
        }

        public enum ObjectType { Cop, Witness, Ped, Vehicle, Barrier }

        public enum PedType { Cop, Ambient }
    }
}