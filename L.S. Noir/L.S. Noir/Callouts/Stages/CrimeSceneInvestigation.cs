using System;
using System.Collections.Generic;
using System.IO;
using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Common.Process;
using LSNoir.Startup;
using Rage;

namespace LSNoir.Callouts.Stages
{
    public class CrimeSceneInvestigation : StageBase
    {
        public CrimeSceneInvestigation(Case caseRef, Stage stage) : base(caseRef, stage)
        {
        }

        private void CrimeSceneInvestigation_OnArrivedAtScene()
        {
            
        }


        public override bool Initialize()
        {
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(Initialize), "Initialize");

            OnArrivedAtScene += CrimeSceneInvestigation_OnArrivedAtScene;

            GameFiber.StartNew(HandleTimedEvent);
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(Initialize), "Initialize completed");
            return true;
        }

        public override void EnRoute()
        {
        }

        protected override void Process()
        {            
            
        }

        private void HandleTimedEvent()
        {
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(HandleTimedEvent), "Checking for timed event");
            if (!CheckForTimedEvent(out var entities)) return;

            TimedEventTime = DateTime.Now + StageRef.TimedEventDelay;
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(HandleTimedEvent), $"Timed event present. Sleeping {(TimedEventTime - DateTime.Now).TotalSeconds} seconds");
            
            GameFiber.Sleep((int)(TimedEventTime - DateTime.Now).TotalMilliseconds);
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(HandleTimedEvent), "Timed event starting");

            SpawnTimedEventEntities();
        }
        
        /// <summary>
        /// Timed Events are where the player has a time limit before an event occurs.  All entities with ID "TIMED_EVENT" are spawned and move to the secondary position.
        /// </summary>
        private bool CheckForTimedEvent(out Dictionary<SceneItem, Entity> timedEventEntities)
        {
            var dynamicEventExists = false;
            timedEventEntities = new Dictionary<SceneItem, Entity>();

            // todo - need to fix case loading to keep specific files so we can pull the directory.  Maybe save that string when loading the case?
            
            // todo - change to a new file for timed events - if that file doesn't exist, this returns false.  File contains necessary data
            
            foreach (var item in SceneItems)
            {
                if (!item.Key.ID.Contains("TIMED_EVENT")) continue;
                dynamicEventExists = true;
                timedEventEntities.Add(item.Key, item.Value);
            }

            return dynamicEventExists;
        }

        private void SpawnTimedEventEntities()
        {
            
        }

        private DateTime TimedEventTime = DateTime.MaxValue;

        private void End()
        {            
            Logger.LogDebug(nameof(CrimeSceneInvestigation), nameof(End), $"Ending");
            base.End(true);
        }
    }
}