using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiskey111Common;
using LSNoir.Extensions;
using Rage;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts.SA
{
    internal static class Evid_War_TimeChecker
    {
        private static GameFiber _fiber;
        private static List<TimeCheckObject> _objList { get; set; }

        internal static void StartChecker()
        {
            _fiber = new GameFiber(CheckFiber);
            _fiber.Start();
            InitialStartup();
        }

        internal static void InitialStartup()
        {
            "Checking for outstanding evidence/warrants".AddLog(true);
            _objList = new List<TimeCheckObject>();

            var cData = LoadItemFromXML<CaseData>(Main.CDataPath);
            if (String.IsNullOrEmpty(cData.StartingStage)) return;

            var eList = LoadItemFromXML<List<EvidenceData>>(Main.EDataPath);

            if (eList.Count > 0)
            {
                foreach (var item in eList)
                {
                    if (item.TestingFinishTime == DateTime.MinValue || item.IsTested) continue;

                    AddObject(new TimeCheckObject(TimeCheckObject.Type.Evidence, item.Name, item.TestingFinishTime));
                }
            }

            if (cData.WarrantSubmitted && !cData.WarrantHeard) AddObject(new TimeCheckObject(TimeCheckObject.Type.Warrant, "Warrant", cData.WarrantApprovedDate));
        }
        
        internal static void AddObject(TimeCheckObject obj)
        {
            $"Adding object {obj.Name} to time check".AddLog(true);
            _objList.Add(obj);
        }

        internal static void RemoveObject(TimeCheckObject obj)
        {
            $"Removing object {obj.Name} to time check".AddLog(true);
            if (_objList.Contains(obj)) _objList.Remove(obj);
        }

        private static void CheckFiber()
        {
            while (true)
            {
                if (_objList.Count < 1)
                {
                    GameFiber.Yield();
                    continue;
                }

                foreach (var obj in _objList.ToList())
                {
                    if (!obj.Exists && DateTime.Compare(obj.CompletionTime, DateTime.Now) < 0) continue;
                    EvidenceTestCompleted(obj);
                }

                GameFiber.Yield();
            }
        }

        private static void EvidenceTestCompleted(TimeCheckObject obj)
        {

            $"{obj.Name} testing completed".AddLog(true);

            if (obj.CheckType == TimeCheckObject.Type.Evidence)
            {
                var eList = LoadItemFromXML<List<EvidenceData>>(Main.EDataPath);
                if (eList.Count < 1) return;

                foreach (var val in eList)
                {
                    if (val.Name != obj.Name && !val.IsTested) continue;
                    val.IsTested = true;
                    "~b~Police Laboratory".DisplayNotification($"Evidence testing ~g~completed~w~ for ~y~{obj.Name}~w~\nView the details in the ~b~SAJRS ~w~computer");
                    break;
                }
                SaveItemToXML(eList, Main.EDataPath);
            }
            else
            {
                "Hearing warrant".AddLog(true);
                var data = LoadItemFromXML<CaseData>(Main.CDataPath);
                data.WarrantHeard = true;
                if (data.WarrantReason == "Gut Feeling" || data.WarrantReason == "None") data.WarrantApproved = MathHelper.GetRandomInteger(10) != 1;
                else data.WarrantApproved = MathHelper.GetRandomInteger(250) != 1;
                SaveItemToXML<CaseData>(data, Main.CDataPath);
            }
            RemoveObject(obj);
        }
    }

    internal class TimeCheckObject
    {
        internal Type CheckType { get; set; }
        internal string Name { get; set; }
        internal DateTime CompletionTime { get; set; }
        internal bool Exists { get; set; }

        internal TimeCheckObject(Type type, string name, DateTime time)
        {
            CheckType = type;
            Name = name;
            CompletionTime = time;
            Exists = true;
        }

        internal static DateTime RandomTimeCreator()
        {
            var timeSpan = DateTime.Now.AddDays(2d) - DateTime.Now;
            var newSpan = new TimeSpan(0, Rand.RandomNumber(0, (int)timeSpan.TotalMinutes), 0);

            var finalTime = DateTime.Now + newSpan;

            $"Time for evidence test completion: {finalTime}".AddLog(true);
            return finalTime;
        }

        internal enum Type { Evidence, Warrant }
    }
}
