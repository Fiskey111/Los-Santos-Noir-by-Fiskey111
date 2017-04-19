/*
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts
{
    public class Sa_2aHospital : BasicScript
    {
        private Blip _hospBlip;
        private Vector3 _position = new Vector3(298.52f, -584.51f, 43.26f);
        private Ped _doc;
        private ELocation _state;
        private EDialog _dialogstate;
        private Object _notepad;
        private Vector3 _playerPos => Game.LocalPlayer.Character.Position;
        private Marker _marker;

        private Dialogue _dialogue;
        private CaseData _cData = LoadItemFromXML<CaseData>(Main.CDataPath);

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 2a [Hospital]".AddLog();
            
            _hospBlip = new Blip(_position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Enter Hospital"
            };

            "Sexual Assault Case Update".DisplayNotification("Visit Hospital for Update");

            _marker = new Marker(_position, Color.Green, Marker.MarkerTypes.MarkerTypeUpsideDownCone, true, false, true);
            ActivateStage(WaitForEnterCommand);
            
            return true;
        }

        private void WaitForEnterCommand()
        {
            if (_playerPos.DistanceTo(_position) > 1.5f) return;
            
            "At hospital; entering".AddLog();

            if (_marker.Exists) _marker.Stop();

            SwapStages(WaitForEnterCommand, CameraAndFade);
        }

        private void CameraAndFade()
        {
            //todo -- interpolate camera to hospital sign
            Game.FadeScreenOut(5000);

            while (Game.IsScreenFadedIn)
                GameFiber.Yield();

            SwapStages(CameraAndFade, LoadHospital);
        }

        private void LoadHospital()
        {
            OpenHospital();
            
            GameFiber.Sleep(0500);

            LoadPeds();

            Game.LocalPlayer.Character.Position = new Vector3(298.52f, -584.51f, 43.27f);

            SwapStages(LoadHospital, FadeScreenIn);
        }

        private void FadeScreenIn()
        {
            Game.FadeScreenIn(2000);

            while (!Game.IsScreenFadedIn)
                GameFiber.Yield();

            SwapStages(FadeScreenIn, WaitForSecretaryTalk);
        }

        private void WaitForSecretaryTalk()
        {
            //todo -- add if (distancetoPed > 2f) return;
        }

        protected override void Process() { }

        private void OpenHospital()
        {
            CallNatives();

            SpawnFloor();

            GameFiber.Sleep(0500);

            LoadPeds();
        }

        private void CallNatives()
        {
            NativeFunction.Natives.x0888C3502DBBEEF5();
            NativeFunction.Natives.x9BAE5AD2508DF078(1);

            NativeFunction.Natives.SET_STREAMING(true);
            NativeFunction.Natives.REQUEST_IPL("RC12B_Default");
            NativeFunction.Natives.REQUEST_IPL("RC12B_Destroyed");
            NativeFunction.Natives.REQUEST_IPL("RC12B_Fixed");
            NativeFunction.Natives.REMOVE_IPL("RC12B_HospitalInterior");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Fixed");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_HospitalInterior");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Default");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Destroyed");

            NativeFunction.Natives.REQUEST_COLLISION_AT_COORD(311.4596f, -588.8196f, 41.3174f);
            var hosp = NativeFunction.CallByName<int>("GET_INTERIOR_AT_COORDS", 311.4596f, -588.8196f, 44.3174f);
            NativeFunction.Natives.x2CA429C029CCF247(hosp);
            NativeFunction.Natives.SET_INTERIOR_ACTIVE(hosp, true);
            NativeFunction.Natives.LOAD_SCENE(330.4596f, -584.8196f, 42.3174f, true);
        }

        private void SpawnFloor()
        {
            var model = new Model("prop_container_01a");

            var q = -583f;
            for (var i = 307f; i > 287f; i = i - 1f)
            {
                var obj = new Object(model, new Vector3(i, q, 39.441f));
                obj.Heading = 252f;
                q = q - 2.5f;
            }

            var x = 319f;
            for (var w = -586f; w > -610f; w = w - 2.5f)
            {
                var obj = new Object(model, new Vector3(x, w, 39.441f));
                obj.Heading = 252f;
                x = x - 1f;
            }

            var x2 = 326f;
            for (var w = -587f; w > -610f; w = w - 2.5f)
            {
                var obj = new Object(model, new Vector3(x2, w, 39.441f));
                obj.Heading = 252f;
                x2 = x2 - 1f;
            }
        }

        private void LoadPeds()
        {
            
        }
        
        protected override void End() { }

        protected void SetScriptFinished()
        {
            _case.LastCompletedStage = CaseData.LastStage.Hospital;
            _case.SajrsUpdates.Add("Hospital Physician Report Added");
            _case.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            CaseSerializer.SaveCaseData(CaseSerializer.DataType.Case, _case);
            "Sexual Assault Case Update".DisplayNotification("Hospital Conversation \nAdded to ~b~SAJRS");
            DeactivateStage(Leaving);
            SetScriptFinished(true);
        }

        public enum ELocation
        {
            Dispatched, Within80, Within15, Within5, Done
        }
        public enum EDialog
        {
            Pre, During, Post
        }
    }
}
*/