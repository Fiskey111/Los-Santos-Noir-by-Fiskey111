using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;

namespace LSNoir.Stages
{
    public class Stakeout : BasicScript
    {
        private readonly StageData _data;

        private const float _offLimitsArea = 45f;
        private Blip _watchAreaBlip, _targetBlip;
        private ISceneActive _scene;

        private string MSG_PARK_VEHICLE = $"Park your vehicle outside the ~y~highlighted area~w~.\nEnsure you still can see the ~o~target~w~ house.\n\nPress ~g~{Settings.Controls.KeyOpenSceneCamera}~w~ when you are in position.";
        private const string LEAVE_AREA = "Leave the area.";
        private const string WARN_SPOTTED = "You may be ~r~spotted~w~ and blow your cover, leave the ~y~highlighted area~w~ now!";
        private const string BEEN_SPOTTED = "~r~You've been spotted. Try again some other time.";

        private const float DIST_CLOSE = 70f;
        private const float DIST_AREA_LEFT = 30f; // todo -- replace with xml val

        private int _enteredAreaCount;
        private bool _sceneCreated;
        
        public static float DistToPlayer(Vector3 e) => Vector3.Distance(Game.LocalPlayer.Character.Position, e);
        
        public Stakeout(StageData stageData)
        {
            // Set the stage data
            _data = stageData;
        }

        protected override bool Initialize()
        {
            // Display our notification
            Base.SharedStageMethods.DisplayNotification(_data);

            // Set the blip to the target area
            _targetBlip = Base.SharedStageMethods.CreateBlip(_data);

            // Flash the minimap
            NativeFunction.Natives.FlashMinimapDisplay();

            // Set ISceneActive
            _scene = Base.SharedStageMethods.GetScene(_data) as ISceneActive;
            
            // Start checking the position
            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if (DistToPlayer(_targetBlip.Position) < 70f && !_sceneCreated)
            {
                // Create the scene and wait for its completion
                _scene?.Create();
                _sceneCreated = true;
            }

            // If the player isn't close, ignore
            if (DistToPlayer(_targetBlip.Position) > DIST_CLOSE) return;

            // Tell the player to park their car
            Game.DisplayHelp(MSG_PARK_VEHICLE, true);

            // Reset the target blip to be the target location
            if (_targetBlip) _targetBlip.Delete();
            _targetBlip = new Blip(_data.CallPosition);
            _targetBlip.Color = Color.Red;

            // Add a no-entry zone
            _watchAreaBlip = new Blip(_targetBlip.Position, _offLimitsArea);
            _watchAreaBlip.Color = Color.Yellow;
                
            // Wait until the player has parked
            SwapStages(Away, HasPlayerParked);
        }

        private void HasPlayerParked()
        {
            // If the player is within the no-entry area, yell at them
            if (DistToPlayer(_watchAreaBlip.Position) < _offLimitsArea)
            {
                // Let the player know, then sleep the fiber and increase the entered count
                Game.DisplaySubtitle(WARN_SPOTTED, 4000);
                GameFiber.Sleep(4000);
                _enteredAreaCount++;
                
                // If the entered count is over this random, fail the stage.
                if (_enteredAreaCount == MathHelper.GetRandomInteger(3, 6))
                {
                    Game.DisplaySubtitle(BEEN_SPOTTED, 4000);
                    SetScriptFinished(false);
                }
            }
            else if (Game.IsKeyDown(Settings.Controls.KeyOpenSceneCamera) && Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.Speed < 0.5f)
            {
                // The player is stopped outside the zone and is ready to start
                Game.HideHelp();

                Game.LocalPlayer.HasControl = false;
                
                SwapStages(HasPlayerParked, StartStakeout);
            }
        }

        private void StartStakeout()
        {
            SceneCamera sceneCamera = new SceneCamera($@"Plugins/LSPDFR/LSNoir/Cases/{_data.ParentCase}/Progress/Photos/Current"); // todo -- make dynamic based on case location and number
            sceneCamera.Start();

            SwapStages(StartStakeout, IsSceneDone);
        }

        private void IsSceneDone()
        {
            // While the scene is being created, wait
            if (!_scene.HasFinished) return;
            
            SwapStages(IsSceneDone, HasPlayerLeftArea);
        }

        private void HasPlayerLeftArea()
        {
            Game.DisplaySubtitle(LEAVE_AREA);

            if (DistToPlayer(_data.CallPosition) > DIST_AREA_LEFT)
            {
                SetScriptFinished(true);
            }
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
            if (_targetBlip) _targetBlip.Delete();
            if (_watchAreaBlip) _watchAreaBlip.Delete();
            _scene?.Dispose();
            MissionSummaryScreen s = new MissionSummaryScreen(_data);
            s.Start();
        }
    }
}
