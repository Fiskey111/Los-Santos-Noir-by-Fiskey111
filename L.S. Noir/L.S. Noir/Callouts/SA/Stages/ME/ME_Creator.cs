using Rage;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using StageObjects;
using LSNoir.Callouts.SA.Commons;
using Rage.Native;
using AnimationDatabase;
using LSNoir.Extensions;
using LtFlash.Common;
using Fiskey111Common;

namespace StageCreator
{
    public class MeCreator
    {
        internal static StageObject P1 = new StageObject(), P2 = new StageObject(), 
            P3 = new StageObject(), P4 = new StageObject(),
            P5 = new StageObject(), P6 = new StageObject(),
            _a1 = new StageObject(), _a2 = new StageObject(), _a3 = new StageObject(), 
            _a4 = new StageObject(), _a5 = new StageObject(), _a6 = new StageObject(),
            MedicalExaminer = new StageObject();

        internal static Vector3 PPos;

        internal static Vector3 A1, A2, A3, A4, A5, A6;

        public static Stopwatch Sw = new Stopwatch();

        internal static GameFiber AmbientFiber = new GameFiber(AmbientPeds);

        internal static List<string> PedList = new List<string>();
        private static List<string> _scenarios = new List<string>();

        internal static Dictionary<SpawnPoint, bool> AmbientList = new Dictionary<SpawnPoint, bool>();

        internal static XDocument Xml;
        internal const string Xmlpath = @"Plugins\LSPDFR\LSNoir\SA\Data\ServiceCoordinates.xml";
        internal static string Root;
        internal static float Zvalue;
        internal static int Number = 1, Tasknum = 1, Location = 1;


        public static void CreateScene(Vector3 playerPosition, Vector3 mePosition)
        {
            "Starting Scene Creation -- Stopwatch Started".AddLog();
            Sw.Start();
            int id = 60418;
            NativeFunction.Natives.SET_INTERIOR_ACTIVE(id, true);
            NativeFunction.Natives.x2CA429C029CCF247(id);
            LoadXml();
            "XML Loaded".AddLog();
            AmbientFiber.Start();
        }

        private static void LoadXml() 
        {
            "Retreiving XML Data".AddLog();
            Xml = XDocument.Load(Xmlpath);
            ("XML Loaded from " + Xmlpath).AddLog();
            
            PedList.Add("s_m_y_autopsy_01");
            PedList.Add("s_m_m_doctor_01");
            PedList.Add("s_f_y_scrubs_01");
            PedList.Add("s_m_m_scientist_01");
            
            Root = "ME";

            ("Root: " + Root).AddLog();

            Zvalue = float.Parse(Xml.Root.XPathSelectElement(Root + "/OverallZ").Value, CultureInfo.InvariantCulture);
                
            ("OverallZ (float) = " + Zvalue.ToString()).AddLog();

            PPos = new Vector3(float.Parse(Xml.Root.XPathSelectElement(Root + "/Pl/x").Value, CultureInfo.InvariantCulture),
                        float.Parse(Xml.Root.XPathSelectElement(Root + "/Pl/y").Value, CultureInfo.InvariantCulture),
                        Zvalue);
            Game.LocalPlayer.Character.Position = PPos;
            Game.LocalPlayer.Character.Heading = float.Parse(Xml.Root.XPathSelectElement(Root + "/Pl/h").Value, CultureInfo.InvariantCulture);


            // STANDING PEDS

            CreateStage_Object(P1, "/P1");

            CreateStage_Object(P2, "/P2");

            CreateStage_Object(P3, "/P3");

            CreateStage_Object(P4, "/P4");

            CreateStage_Object(P5, "/P5");

            CreateStage_Object(P6, "/P6");

            CreateStage_Object(MedicalExaminer, "/ME", true);
            
            CreateStage_Object(_a1, "/A1");

            CreateStage_Object(_a2, "/A2");

            CreateStage_Object(_a3, "/A3");

            CreateStage_Object(_a4, "/A4");

            CreateStage_Object(_a5, "/A5");

            CreateStage_Object(_a6, "/A6");
        }

        private static void CreateStage_Object(StageObject obj, string location, bool force = false)
        {
            SpawnPoint spawn = new SpawnPoint(float.Parse(Xml.Root.XPathSelectElement(Root + location + "/h").Value, CultureInfo.InvariantCulture),
                float.Parse(Xml.Root.XPathSelectElement(Root + location + "/x").Value, CultureInfo.InvariantCulture),
                        float.Parse(Xml.Root.XPathSelectElement(Root + location + "/y").Value, CultureInfo.InvariantCulture),
                        Zvalue);
            if (MathHelper.GetRandomInteger(2) == 1 || force)
            {
                string model = (string)PedList[Rand.RandomNumber(1, PedList.Count)];

                obj.Ped = new Ped(model, spawn.Position, spawn.Heading);
                obj.Ped.RandomizeVariation();
                obj.Ped.MakeMissionPed();
                obj.Exists = true;

                if (!force)
                {
                    AmbientList.Add(spawn, true);
                    "Ped added to ambient list".AddLog();
                }
                
                ("Stage_Object at location " + location + " created successfully").AddLog();
            }
            else
            {
                obj.Exists = false;
                AmbientList.Add(spawn, false);
                ("Stage_Object " + Number + " skipped").AddLog();
            }
            Number++;
        }
                
        private static void AmbientPeds()
        {
            _scenarios.Add("WORLD_HUMAN_AA_COFFEE");
            _scenarios.Add("WORLD_HUMAN_DRINKING");
            _scenarios.Add("WORLD_HUMAN_STAND_IMPATIENT");
            _scenarios.Add("WORLD_HUMAN_HANG_OUT_STREET");
            _scenarios.Add("WORLD_HUMAN_STAND_MOBILE");
            _scenarios.Add("WORLD_HUMAN_CLIPBOARD");

            Tasknum = 1;

            StartScenario(_a1, true);
            Tasknum++;

            StartScenario(_a2, true);
            Tasknum++;

            StartScenario(_a3, true);
            Tasknum++;

            StartScenario(_a4, true);
            Tasknum++;

            StartScenario(_a5, true);
            Tasknum++;

            StartScenario(_a6, true);
            Tasknum++;

            StartScenario(P1);
            Tasknum++;

            StartScenario(P2);
            Tasknum++;

            StartScenario(P3);
            Tasknum++;

            StartScenario(P4);
            Tasknum++;

            StartScenario(P5);
            Tasknum++;

            StartScenario(P6);
            Tasknum++;
        }

        private static void StartScenario(StageObject obj, bool movement = false)
        {
            if (obj.Exists)
            {
                ("Starting task for object " + Tasknum).AddLog();
                GameFiber.StartNew(delegate
                {
                    if (!movement)
                    {
                        while (true)
                        {
                            GameFiber.Yield();

                            string scenar = _scenarios[MathHelper.GetRandomInteger(_scenarios.Count - 1)];
                            $"Scenario {scenar} assigned for {Tasknum}".AddLog();

                            obj.Ped.Task_Scenario(scenar);
                            while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(obj.Ped))
                            {
                                GameFiber.Yield();
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            int pnum = Tasknum;
                            GameFiber.Yield();

                            GameFiber.Sleep(MathHelper.GetRandomInteger(1000, 20000));
                            int random = MathHelper.GetRandomInteger(3);
                            if (random == 1)
                            {
                                ("Wandering ped " + Tasknum).AddLog();
                                obj.Ped.Tasks.Wander();
                            }
                            else if (random == 2)
                            {
                                ("Moving ped " + Tasknum).AddLog();
                                SpawnPoint loc = MovementLocation();
                                obj.Ped.Tasks.GoStraightToPosition(loc.Position, 1f, loc.Heading, 0f, -1);
                                while (obj.Ped.DistanceTo(loc.Position) < 1f)
                                {
                                    GameFiber.Yield();
                                    continue;
                                }

                                ("Creating scenario for ped " + Tasknum + " after moving").AddLog();
                                while (true)
                                {
                                    GameFiber.Yield();
                                    obj.Ped.Task_Scenario(_scenarios[MathHelper.GetRandomInteger(_scenarios.Count - 1)]);
                                    while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(obj.Ped))
                                    {
                                        GameFiber.Yield();
                                    }
                                }
                            }
                            else
                            {
                                ("Creating scenario for ped " + Tasknum).AddLog();
                                while (true)
                                {
                                    GameFiber.Yield();
                                    obj.Ped.Task_Scenario(_scenarios[MathHelper.GetRandomInteger(_scenarios.Count - 1)]);
                                    while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(obj.Ped))
                                    {
                                        GameFiber.Yield();
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        private static SpawnPoint MovementLocation()
        {
            SpawnPoint sp = new SpawnPoint(0, 0, 0, 0);
            foreach (SpawnPoint spawn in AmbientList.Keys.ToList())
            {
                if (AmbientList[spawn])
                {
                    continue;
                }

                sp = spawn;
                AmbientList[spawn] = true;
            }

            return sp;
        }
    }
}
