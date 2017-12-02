using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System.Drawing;
using System.Linq;
using Gwen.Anim;

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
        //SCENARIO: WORLD_HUMAN_BINOCULARS

        private const string MODEL_CAMERA = "prop_pap_camera_01";
        //CAMERA:
        //ENTITY::ATTACH_ENTITY_TO_ENTITY(l_902, PLAYER::PLAYER_PED_ID(), PED::GET_PED_BONE_INDEX(PLAYER::PLAYER_PED_ID(), 28422), 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1, 1, 0, 0, 2, 1);

        //anim dic: amb@world_human_binoculars@male@idle_a
        //name: idle_a

        public static float DistToPlayer(Vector3 e) => Vector3.Distance(Game.LocalPlayer.Character.Position, e);

        private CameraInterpolator ci;
        private Rage.Object optics;

        public VantagePointObservation(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            Base.SharedStageMethods.DisplayNotification(data);

            blipVantagePoint = Base.SharedStageMethods.CreateBlip(data);

            NativeFunction.Natives.FlashMinimapDisplay();

            scene = Base.SharedStageMethods.GetScene(data) as ISceneActive;
            
            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if(DistToPlayer(blipVantagePoint.Position) < DIST_CLOSE)
            {
                Game.DisplayHelp(MSG_FIND_POINT);

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
            if(DistToPlayer(markerVantagePoint.Position) < DIST_MARKER_ACTIVE)
            {
                if(blipVantagePoint) blipVantagePoint.Delete();
                markerVantagePoint?.Dispose();

                Game.LocalPlayer.HasControl = false;

                var playerData = data.ParentCase.GetPersonData(data.PersonsID.First());
                
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

            Game.LocalPlayer.Character.Tasks.PlayAnimation("amb@world_human_binoculars@male@idle_a", "idle_a", 1, AnimationFlags.Loop);

            ActivateStage(HideHUDAndMap);

            ci = new CameraInterpolator();

            var camPos = Game.LocalPlayer.Character.GetOffsetPositionFront(6);

            ci.Start(camPos, Game.LocalPlayer.Character);

            SwapStages(AttachProps, IsCamDoneInterpolating);
        }

        private void HideHUDAndMap()
        {
            NativeFunction.Natives.HIDE_HUD_AND_RADAR_THIS_FRAME();
        }

        private void IsCamDoneInterpolating()
        {
            if(ci.DoneInterpolating)
            {
                Game.FadeScreenOut(2000, true);

                ci.Stop();

                Game.LocalPlayer.Character.Tasks.ClearImmediately();

                scene.Create();

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
                SetScriptFinished(true);
            }
        }
        
        protected override void Process()
        {
        }

        protected override void End()
        {
            if (optics) optics.Delete();
            if (blipVantagePoint) blipVantagePoint.Delete();
            markerVantagePoint?.Dispose();
            scene?.Dispose();
        }
    }
}
