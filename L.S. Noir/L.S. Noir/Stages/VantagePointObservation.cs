using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System.Drawing;
using System.Linq;

namespace LSNoir.Stages
{
    class VantagePointObservation : BasicScript
    {
        private readonly StageData data;

        private Blip blipVantagePoint;
        private Marker markerVantagePoint;
        private ISceneActive scene;

        private const string MSG_FIND_POINT = "Find a ~y~vantage point~s~.";
        private const string LEAVE_AREA = "Leave the area.";
        private const float DIST_CLOSE = 30f;
        private const float DIST_MARKER_ACTIVE = 1.5f;
        private const float DIST_AREA_LEFT = 30f; //replace with xml val

        private const string MODEL_BINOCULARS = "v_serv_ct_binoculars";
        private const string SCENARIO_BINOCULARS = "WORLD_HUMAN_BINOCULARS";

        private const string MODEL_CAMERA = "prop_pap_camera_01";

        private const string PLAYER_DATA = "obs_player";
        //CAMERA:
        //ENTITY::ATTACH_ENTITY_TO_ENTITY(l_902, PLAYER::PLAYER_PED_ID(), PED::GET_PED_BONE_INDEX(PLAYER::PLAYER_PED_ID(), 28422), 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1, 1, 0, 0, 2, 1);

        //anim dic: amb@world_human_binoculars@male@idle_a
        //name: idle_a

        public static float DistToPlayer(Vector3 e) => Vector3.Distance(Game.LocalPlayer.Character.Position, e);

        private CameraInterpolator camInterpolator;
        private Rage.Object optics;

        private RouteAdvisor ra;

        public VantagePointObservation(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            data.CallNotification.DisplayNotification();

            blipVantagePoint = Base.SharedStageMethods.CreateBlip(data);

            NativeFunction.Natives.FlashMinimapDisplay();

            scene = Base.SharedStageMethods.GetScene(data) as ISceneActive;

            ra = new RouteAdvisor(data.CallPosition);

            ra.Start(false, true);

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if(DistToPlayer(blipVantagePoint.Position) < DIST_CLOSE)
            {
                ra.Stop();

                blipVantagePoint.Delete();

                blipVantagePoint = new Blip(data.CallPosition);
                blipVantagePoint.Color = Color.Yellow;

                markerVantagePoint = new Marker(data.CallPosition, Color.Yellow);
                markerVantagePoint.Visible = true;

                SwapStages(Away, HasPlayerEnterMarker);
            }
        }

        private void HasPlayerEnterMarker()
        {
            Game.DisplaySubtitle(MSG_FIND_POINT);

            if(DistToPlayer(markerVantagePoint.Position) < DIST_MARKER_ACTIVE)
            {
                if(blipVantagePoint) blipVantagePoint.Delete();
                markerVantagePoint?.Dispose();

                Game.LocalPlayer.HasControl = false;

                var playerData = data.GetResourceByName<PersonData>(PLAYER_DATA);
                
                var pos = playerData.Spawn.Position;

                var heading = playerData.Spawn.Heading;

                var task = Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(pos, heading, 1.4f);

                task.WaitForCompletion();

                SwapStages(HasPlayerEnterMarker, AttachProps);
            }
        }
        
        private void AttachProps()
        {
            optics = new Rage.Object(MODEL_BINOCULARS, data.CallPosition);

            optics.AttachTo(Game.LocalPlayer.Character, (int)PedBoneId.RightHand, Vector3.Zero, Rotator.Zero);

            NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(Game.LocalPlayer.Character, SCENARIO_BINOCULARS, 0, true);

            ActivateStage(HideHUDAndMap);

            camInterpolator = new CameraInterpolator();

            var camPos = Game.LocalPlayer.Character.GetOffsetPositionFront(6);

            camInterpolator.Start(camPos, Game.LocalPlayer.Character);

            SwapStages(AttachProps, IsCamDoneInterpolating);
        }

        private void HideHUDAndMap()
        {
            NativeFunction.Natives.HIDE_HUD_AND_RADAR_THIS_FRAME();
        }

        private void IsCamDoneInterpolating()
        {
            if(camInterpolator.DoneInterpolating)
            {
                Game.FadeScreenOut(2000, true);

                camInterpolator.Stop();

                Game.LocalPlayer.Character.Tasks.ClearImmediately();

                scene.Create();
                scene.Start();

                Game.FadeScreenIn(2000, true);

                SwapStages(IsCamDoneInterpolating, IsSceneDone);
            }
        }

        private void IsSceneDone()
        {
            if(scene.HasFinished)
            {
                if(optics) optics.Delete();

                DeactivateStage(HideHUDAndMap);
                
                Game.LocalPlayer.HasControl = true;

                Game.FadeScreenIn(2000, true);

                SwapStages(IsSceneDone, HasPlayerLeftArea);
            }
        }

        private void HasPlayerLeftArea()
        {
            Game.DisplaySubtitle(LEAVE_AREA);

            if(DistToPlayer(data.CallPosition) > DIST_AREA_LEFT)
            {
                SetScriptFinishedAndSave();
            }
        }

        private void SetScriptFinishedAndSave()
        {
            data.ParentCase.Progress.SetLastStage(data.ID);
            if (data.NextScripts != null && data.NextScripts.Count > 0)
            {
                data.ParentCase.Progress.SetNextScripts(data.NextScripts[0]);
            }

            SetScriptFinished(true);
        }
        
        protected override void Process()
        {
        }

        protected override void End()
        {
            ra?.Stop();

            if (optics) optics.Delete();
            if (blipVantagePoint) blipVantagePoint.Delete();
            markerVantagePoint?.Dispose();
            scene?.Dispose();
        }
    }
}
