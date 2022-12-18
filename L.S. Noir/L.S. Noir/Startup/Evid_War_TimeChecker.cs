using System;
using System.Collections.Generic;
using System.Linq;
using LSNoir.Common;
using LSNoir.Extensions;
using Rage;
using Random = LSNoir.Common.Random;

namespace LSNoir.Startup
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
            Game.AddConsoleCommands();
        }

        internal static void InitialStartup()
        {
            try
            {
                "Checking for outstanding evidence/warrants".AddLog(true);
                _objList = new List<TimeCheckObject>();

                /*
                foreach (var _case in Main.InternalCaseManager.CaseList)
                {
                    foreach (var stage in _case.LoadedCase.Stages)
                    {
                        foreach (var entity in stage.InteractiveEntities.Where(entity =>
                                     entity.InteractionSettings.Evidence.TestCompletionTime != DateTime.MinValue))
                        {
                            AddObject(new TimeCheckObject(TimeCheckObject.Type.Evidence, entity.Description,
                                entity.InteractionSettings.Evidence.TestCompletionTime));
                        }
                    }

                    foreach (var document in _case.LoadedCase.Progress.DocumentsRequested)
                    {
                        foreach (var doc in _case.LoadedCase.Documents)
                        {
                            if (doc.ID != document) continue;
                            AddObject(new TimeCheckObject(TimeCheckObject.Type.Warrant, doc.Messages.Title,
                                doc.DocumentCompletionTime));
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                Logger.LogDebug(nameof(Evid_War_TimeChecker), nameof(InitialStartup), ex.ToString());
            }
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

        internal static void SkipWaitTimes()
        {
            "Skipping wait time".AddLog(true);

            if (_objList.Count < 1)
            {
                "No objects to skip wait time for".AddLog(true);
                return;
            }
            foreach (var obj in _objList.ToArray())
            {
                $"Skipping test for object {obj.Name}".AddLog(true);
                EvidenceTestCompleted(obj);
            }
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

                foreach (var obj in _objList.ToArray())
                {
                    if (!obj.Exists) continue;
                    if (DateTime.Compare(obj.CompletionTime, DateTime.Now) < 0) continue;
                    EvidenceTestCompleted(obj); 
                }

                GameFiber.Yield();
            }
        }

        private static void EvidenceTestCompleted(TimeCheckObject obj)
        {

            $"{obj.Name} testing completed; datetime.compare = {DateTime.Compare(obj.CompletionTime, DateTime.Now)}".AddLog(true);

/*
if (obj.CheckType == TimeCheckObject.Type.Evidence)
{
foreach (var _case in Main.InternalCaseManager.CaseList)
{
    foreach (var stage in _case.LoadedCase.Stages)
    {
        foreach (var entity in stage.InteractiveEntities.Where(entity =>
                     entity.InteractionSettings.Evidence.TestCompletionTime != DateTime.MinValue))
        {
            if (obj.Name != entity.Description) continue;
            entity.InteractionSettings.Evidence.TestCompleted = true;
            "~b~Police Laboratory".DisplayNotification($"Evidence testing ~g~completed~w~ for ~y~{obj.Name}~w~\nView the details in the ~b~SAJRS ~w~computer", _case.LoadedCase.Progress.CaseNumber);
            break;
        }
    }
}
}
else
{
foreach (var _case in Main.InternalCaseManager.CaseList)
{
    foreach (var document in _case.LoadedCase.Progress.DocumentsRequested)
    {
        foreach (var doc in _case.LoadedCase.Documents)
        {
            if (doc.ID != document) continue;
            _case.LoadedCase.Progress.DocumentsAccepted.Add(doc.ID);
            _case.LoadedCase.Progress.DocumentsRequested.Remove(doc.ID);
        }
    }
}
}*/
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
var testTime = Settings.Settings.TestTimes();
var timeSpan = DateTime.Now.Add(new TimeSpan(testTime.Days, testTime.Hours, testTime.Minutes, 0)) - DateTime.Now;

var newSpan = new TimeSpan(0, Random.RandomInt(0, (int)timeSpan.TotalMinutes), 0);

var finalTime = DateTime.Now + newSpan;

$"Time for test completion: {finalTime}".AddLog(true);
return finalTime;
}

internal enum Type { Evidence, Warrant }
}
}
