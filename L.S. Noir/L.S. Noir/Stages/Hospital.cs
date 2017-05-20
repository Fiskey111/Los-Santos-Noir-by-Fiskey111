using LSNoir.Data;
using LSNoir.Scenes;
using LtFlash.Common;
using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System.Drawing;

namespace LSNoir.Stages
{
    public class Hospital : BasicScript
    {
        //TODO:
        // - SceneData might store peds positions inside the hospital
        // - use WitnessData to create doc
        // - try..catch for each of in-hospital stages to tp back on exception
        // - SceneData: add CallBlipColor
        // - move model doc to witnessData?

        //TECHNICAL REQUIREMENTS:
        // - data.DialogsId contains 1 dialog between player and a doctor
        // - SceneData might contain prop peds inside hospital
        // - data.CallPosition marks the place where player can enter a hospital
        // - evidence might be added to case progress

        private readonly StageData data;

        private Ped doctor;
        private readonly static SpawnPoint posDoctorStart = new SpawnPoint(15.6980171f, new Vector3(308.876556f, -596.9119f, 43.26168f));
        private const string MODEL_DOCTOR = "s_m_m_doctor_01";
        private readonly SpawnPoint spawnInsideHospital = new SpawnPoint(245, new Vector3(300, -585, 43));

        private IDialog dialog;
        private IScene scene;

        private Blip blipHospital;
        private Marker markerExit;
        private Marker markerEntrance;

        private Ped Player => Game.LocalPlayer.Character;
        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);

        private const string MSG_LEAVE = "You may ~r~leave~s~ the hospital";
        private const string MSG_ENTER_HOSPITAL = "Enter the ~g~marker~s~ to talk to a doctor";
        private const string MSG_FINISHED = "Stage was successfuly fisnished!";


        public Hospital(StageData stageData)
        {
            data = stageData;
            var sceneId = data.SceneID;
            var sceneData = data.ParentCase.GetSceneData(sceneId);
            scene = new SceneHospitalInterior(sceneData);
        }

        protected override bool Initialize()
        {
            blipHospital = new Blip(data.CallPosition)
            {
                Sprite = data.CallBlipSprite,
                Color = Color.DarkOrange,
                Name = data.CallBlipName,
            };

            markerEntrance = new Marker(data.CallPosition, Color.Green);

            markerEntrance.Visible = true;

            Game.DisplayNotification(data.NotificationTexDic, data.NotificationTexName,
                data.NotificationTitle, data.NotificationSubtitle, data.NotificationText);

            ActivateStage(NotifyToEnterHospital);

            return true;
        }

        private void NotifyToEnterHospital()
        {
            if(DistToPlayer(data.CallPosition) < 15)
            {
                Game.DisplayHelp(MSG_ENTER_HOSPITAL, 3000);

                SwapStages(NotifyToEnterHospital, WaitForEnter);
            }
        }

        private void WaitForEnter()
        {
            if (DistToPlayer(data.CallPosition) < 1.5)
            {
                markerEntrance.Dispose();

                Game.LocalPlayer.HasControl = false;

                SwapStages(WaitForEnter, CameraAndFade);
            }
        }
        private void CameraAndFade()
        {
            //todo -- interpolate camera to hospital sign
            //Resources.CameraInterpolator c = new Resources.CameraInterpolator();
            // position to interpolate to depends on the player location!

            //c.Start();

            Game.FadeScreenOut(2000, true);

            //c.Stop();

            SwapStages(CameraAndFade, LoadHospital);
        }

        private void LoadHospital()
        {
            GameFiber.Sleep(0500);

            scene.Create();

            doctor = new Ped(MODEL_DOCTOR, posDoctorStart.Position, posDoctorStart.Heading);

            var dialogId = data.DialogsID[0];

            var dialogData = data.ParentCase.GetDialogData(dialogId);

            dialog = new Dialog(dialogData.Dialog);

            Player.Position = spawnInsideHospital.Position;

            Player.Heading = spawnInsideHospital.Heading;

            Game.FadeScreenIn(2000, true);

            Game.LocalPlayer.HasControl = true;

            //TODO: stage/method NotifyPlayerToTalkToDoc

            SwapStages(LoadHospital, StartDialogWithDoctor);
        }

        //private void WaitForSecretaryTalk()
        //{
        //    //todo -- add if (distancetoPed > 2f) return;
        //}

        private void StartDialogWithDoctor()
        {
            if(DistToPlayer(doctor.Position) < 2)
            {
                dialog.StartDialog();

                SwapStages(StartDialogWithDoctor, IsDialogFinished);
            }
        }

        private void IsDialogFinished()
        {
            if(dialog.HasEnded)
            {
                Game.DisplayNotification(MSG_LEAVE);

                markerExit = new Marker(spawnInsideHospital.Position, Color.Red);

                markerExit.Visible = true;

                SwapStages(IsDialogFinished, PlayerExit);
            }
        }

        private void PlayerExit()
        {
            if(DistToPlayer(spawnInsideHospital.Position) < 1.5)
            {
                markerExit.Dispose();

                Game.LocalPlayer.HasControl = false;

                Game.FadeScreenOut(2000, true);

                Player.Position = data.CallPosition;

                Game.FadeScreenIn(2000, true);

                Game.LocalPlayer.HasControl = true;

                Game.DisplayNotification(MSG_FINISHED);

                SetScriptFinishedSuccessfulyAndSave();
            }
        }

        protected override void Process()
        {
        }

        private void SetScriptFinishedSuccessfulyAndSave()
        {
            data.ParentCase.AddNotesToProgress(data.NotesID);
            data.ParentCase.AddReportsToProgress(data.ReportsID);
            data.ParentCase.AddEvidenceToProgress(data.EvidenceID);
            data.SetThisAsLastStage();

            SetScriptFinished(true);
        }

        protected override void End()
        {
            if (markerEntrance != null) markerEntrance.Dispose();
            if (markerExit != null) markerExit.Dispose();
            if (doctor) doctor.Delete();
            if (blipHospital) blipHospital.Delete();

            scene.Dispose();
        }
    }
}
