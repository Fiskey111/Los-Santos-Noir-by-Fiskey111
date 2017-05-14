/*
<Hospital>
    <Gen1>
        <x>306.79</x>
        <y>-588.25</y>
        <z>43.26</z>
        <h>343.55</h>
    </Gen1>
    <Gen2>
        <x>316.58</x>
        <y>-587.66</y>
        <z>43.26</z>
        <h>184.24</h>
    </Gen2>
    <Gen3>
        <x>315.28</x>
        <y>-588.87</y>
        <z>43.26</z>
        <h>63.41</h>
    </Gen3>
    <Gen4>
        <x>325.40</x>
        <y>-598.77</y>
        <z>43.26</z>
        <h>250.67</h>
    </Gen4>



    <Nurse1>
        <x>322.96</x>
        <y>-588.80</y>
        <z>43.26</z>
        <h>162.83</h>
    </Nurse1>
    <Nurse2>
        <x>321.80</x>
        <y>-589.36</y>
        <z>43.26</z>
        <h>268.62</h>
    </Nurse2>
    <Nurse3>
        <x>305.69</x>
        <y>-597.87</y>
        <z>43.26</z>
        <h>22.62</h>
    </Nurse3>





    <Doc1>
        <x>316.63</x>
        <y>-597.66</y>
        <z>43.26</z>
        <h>311.61</h>
    </Doc1>
    <DialogDocStart>
        <x>308.13</x>
        <y>-602.30</y>
        <z>43.26</z>
        <h>355.59</h>
    </DialogDocStart>
    <DialogDocEnd>
        <x>308.09</x>
        <y>-596.87</y>
        <z>43.26</z>
        <h>13.97</h>
    </DialogDocEnd>
    <Cashier>
        <x>312.55</x>
        <y>-587.97</y>
        <z>43.26</z>
        <h>348.21</h>
    </Cashier>
    <Enter>
        <x>298.52</x>
        <y>584.51</y>
        <z>43.26</z>
        <h>0.0</h>
    </Enter>
    <Exit>
        <x>300.31</x>
        <y>585.13</y>
        <z>43.26</z>
        <h>0.0</h>
    </Exit>
  </Hospital>

  Gens are ambient peds, cashier is the gift shop person, docstart is where he spawns and then walks to docend to talk to the player, enter and exit are where the player goes to tp

*/


using LSNoire.Data;
using LSNoire.Scenes;
using LtFlash.Common;
using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Stages
{
    public class Hospital : BasicScript
    {
        //TODO:
        // - SceneData might store peds positions inside the hospital
        // - use WitnessData to create doc
        // - try..catch for each of in-hospital stages to tp back on exception
        // - SceneData: add CallBlipColor

        //TECHNICAL REQUIREMENTS:
        // - data.DialogsId contains 1 dialog between player and a doctor
        // - SceneData might contain prop peds inside hospital
        // - data.CallPosition marks the place where player can enter a hospital

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

                SetScriptFinished(true);
            }
        }

        protected override void Process()
        {
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
