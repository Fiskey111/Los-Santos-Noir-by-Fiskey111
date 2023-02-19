using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Common.Process;
using LSNoir.Extensions;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Object = Rage.Object;

namespace LSNoir.Callouts.Stages
{
    public abstract class StageBase
    {
        public  Case CaseRef;
        public  Stage StageRef;
        private ProcessHost _fiber;
        private const float _distanceToCase = 100f;

        private CalloutStage _currentStage;

        public readonly List<Entity> RootEntityList = new List<Entity>();
        private readonly Dictionary<SceneItem, Ped> _scenarioList = new Dictionary<SceneItem, Ped>();

        public readonly Dictionary<SceneItem, Entity> SceneItems = new Dictionary<SceneItem, Entity>();
        public readonly Dictionary<SceneItem, Entity> InteractionItems = new Dictionary<SceneItem, Entity>();

        private readonly Dictionary<SceneItem, Entity> _inspectedList = new Dictionary<SceneItem, Entity>();
        private readonly Dictionary<SceneItem, Entity> _nearbyList = new Dictionary<SceneItem, Entity>();

        public bool IsRunning { get; private set; }
        protected StageBase(Case caseRef, Stage stageRef)
        {
            Logger.LogDebug(nameof(StageBase), nameof(StageBase), "ctor");
            StageRef = stageRef;
            CaseRef = caseRef;

            if (CaseRef == null) throw new NullReferenceException("Case is null");
            if (StageRef == null) throw new NullReferenceException("Stage is null");

            LSPD_First_Response.Mod.API.Functions.SetPlayerAvailableForCalls(false);

            InternalInitialize();
        }

        private void InternalInitialize()
        {
            Logger.LogDebug(nameof(StageBase), nameof(InternalInitialize), "Initializing");
            _currentStage = CalloutStage.Initialize;
            IsRunning = true;

            SpawnSceneItems(StageRef.SceneItems);
            Logger.LogDebug(nameof(StageBase), nameof(InternalInitialize), "Scene items spawned");

            _fiber = new ProcessHost();
            _fiber.StartProcess(Handler);
            _fiber.Start();

            var host = new ProcessHost();
            host.Start();

            host[InteractCheck] = true;
            host[ScenarioPedProcess] = true;
            host[NearbyCheck] = true;
        }

        private void SpawnSceneItems(List<SceneItem> sceneItems)
        {
            foreach (var sceneItem in sceneItems)
            {
                GameFiber.Yield();
                Logger.LogDebug(nameof(StageBase), nameof(SpawnSceneItems), $"Spawning scene item: {sceneItem.ID}");
                var success = sceneItem.SpawnItem(out var entity);
                if (!success)
                {
                    Logger.LogDebug(nameof(StageBase), nameof(SpawnSceneItems), $"Failed to spawn item {sceneItem.ID}");
                    continue;
                }

                if (sceneItem.Scenarios.Count > 0) _scenarioList.Add(sceneItem, entity as Ped);

                if (sceneItem.IsInteractable) CheckForInteractiveEntity(sceneItem, entity);
                
                RootEntityList.Add(entity);
                SceneItems.Add(sceneItem, entity);
            }
        }

        private void ScenarioPedProcess()
        {
            foreach (var item in _scenarioList.ToList())
            {
                GameFiber.Yield();
                if (NativeFunction.Natives.IsPedUsingAnyScenario(item.Value))
                {
                    Logger.LogDebug(nameof(StageBase), nameof(ScenarioPedProcess), $"Skipping ped {item.Key.ID}");
                    continue;
                }

                item.Value.Task_Scenario(item.Key.Scenarios.PickRandom());
            }
        }

        private void CheckForInteractiveEntity(SceneItem sceneItem, Entity entity)
        {
            Logger.LogDebug(nameof(StageBase), nameof(CheckForInteractiveEntity), $"Checking entity: {sceneItem.ID}");

            if (sceneItem.InteractionSettings.CanBeInspected)
            {
                sceneItem.Interacted += EntityOnInteracted;
                _inspectedList.Add(sceneItem, entity);
            }

            if (sceneItem.InteractionSettings.PlaySoundNearby)
            {
                sceneItem.Nearby += EntityOnNearby;
                _nearbyList.Add(sceneItem, entity);
            }
            InteractionItems.Add(sceneItem, entity);
        }

        private void NearbyCheck()
        {
            foreach (var entity in _inspectedList.ToList())
            {
                var position = entity.Key.SpawnPosition.Position;
                if (position.DistanceTo(Game.LocalPlayer.Character) < 1.5f)
                {
                    Logger.LogDebug(nameof(StageBase), nameof(NearbyCheck), $"Entity OnNearby: {entity.Key.ID}");
                    entity.Key.OnNearby();
                    entity.Key.InteractionSettings.HasSoundPlayed = true;
                }
                else if (entity.Key.InteractionSettings.HasSoundPlayed)
                {
                    entity.Key.InteractionSettings.HasSoundPlayed = false;
                }
                GameFiber.Yield();
            }
        }

        private void InteractCheck()
        {
            if (!Game.IsKeyDown(Settings.Settings.InteractKey())) return;

            foreach (var entity in _inspectedList.ToList())
            {
                var position = entity.Key.SpawnPosition.Position;
                if (position.DistanceTo(Game.LocalPlayer.Character) < 1.5f)
                {
                    Logger.LogDebug(nameof(StageBase), nameof(InteractCheck), $"Entity interacted: {entity.Key.ID}");
                    entity.Key.OnInteract();
                }
                GameFiber.Yield();
            }
        }

        private void EntityOnNearby(SceneItem sender)
        {
            Logger.LogDebug(nameof(StageBase), nameof(EntityOnNearby), $"EntityOnNearby: {sender.ID}");
        }

        private void EntityOnInteracted(SceneItem sender)
        {
            Logger.LogDebug(nameof(StageBase), nameof(EntityOnInteracted), $"EntityOnInteracted: {sender.ID}");
        }

        private bool _displayed = false;
        internal void InternalEnRoute()
        {
            if (!_displayed)
            {
                CreateCallBlip(StageRef.CallBlip);

                CreateCallNotification(StageRef.CallNotification);
                _displayed = true;
            }

            if (Game.LocalPlayer.Character.DistanceTo(StageRef.CallPosition.Position) > _distanceToCase)
            {
                EnRoute();
                return;
            }

            Logger.LogDebug(nameof(StageBase), nameof(InternalEnRoute), $"Player arrived at case location");
            OnArrivedAtScene?.Invoke();
            _callBlip?.Delete();
            _currentStage = CalloutStage.Process;
        }

        private void CreateCallBlip(BlipData blipData)
        {
            Logger.LogDebug(nameof(StageBase), nameof(CreateCallBlip), $"Creating blip: {StageRef.CallPosition}");
            _callBlip = new Blip(new Vector3(StageRef.CallPosition.Position.X, StageRef.CallPosition.Position.Y, StageRef.CallPosition.Position.Z), blipData.BlipRadius);
            if (blipData.BlipSprite > 0) _callBlip.Sprite = (BlipSprite)blipData.BlipSprite;
          
            _callBlip.Color = Color.FromArgb(blipData.BlipColor.A, blipData.BlipColor.R, blipData.BlipColor.G,
                blipData.BlipColor.B);

            if (_callBlip.Alpha > 0) _callBlip.Alpha = blipData.BlipColor.A;
            _callBlip.Name = blipData.BlipName;
        }

        private void CreateCallNotification(NotificationData notificationData)
        {
            NotificationExtensionMethods.DisplayNotification(notificationData.Subtitle, notificationData.Text, CaseRef.Progress.CaseNumber);
        }

        private Blip _callBlip;

        /// <summary>
        /// Called initially when the callout is started
        /// </summary>
        /// <returns>True if the callout is valid and can continue<para>False if the callout failed initialization and should end</para></returns>
        public abstract bool Initialize();
        /// <summary>
        /// Called every 250ms until the player is within 20m
        /// </summary>
        public abstract void EnRoute();

        public delegate void ArrivedAtScene();
        public event ArrivedAtScene OnArrivedAtScene;

        protected abstract void Process();

        public virtual void End(bool isFinished)
        {
            Logger.LogDebug(nameof(StageBase), nameof(End), $"Ending stage, completed: {isFinished}");
            IsRunning = false;

            foreach (var ent in RootEntityList)
            {
                ent?.Dismiss();
            }

            if (isFinished)
            {
                StageRef.Completed = true;
            }

            LSPD_First_Response.Mod.API.Functions.SetPlayerAvailableForCalls(true);
        }
        private enum CalloutStage { Initialize, EnRoute, Process, End }

        private void Handler()
        {
            switch (_currentStage)
            {
                case CalloutStage.Initialize:
                    Logger.LogDebug(nameof(StageBase), nameof(Handler), $"Stage: {_currentStage}");
                    var valid = Initialize();
                    if (!valid)
                    {
                        Logger.LogDebug(nameof(StageBase), nameof(Handler), $"Initialize {_currentStage} false");
                        End(false);
                        break;
                    }
                    Logger.LogDebug(nameof(StageBase), nameof(Handler), $"Initialize {_currentStage} true");
                    _currentStage = CalloutStage.EnRoute;
                    break;
                case CalloutStage.EnRoute:
                    InternalEnRoute();
                    break;
                case CalloutStage.Process:
                    Process();
                    break;
            }
        }
    }
}