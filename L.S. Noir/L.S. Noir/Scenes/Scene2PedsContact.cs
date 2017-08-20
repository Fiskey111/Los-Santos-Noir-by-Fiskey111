using LSNoir.Data;
using LtFlash.Common.InputHandling;
using LtFlash.Common.Processes;
using Rage;
using Rage.Native;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Scenes
{
    class Scene2PedsContact : SceneBase, ISceneActive
    {
        public bool HasFinished { get; private set; }

        private readonly SceneData data;
        private Ped pedWalking;
        private Vector3 pedWalkDest;
        private Ped pedStanding;
        private string pedStandingScenario;
        private Task taskGotoPed;
        private Task taskGetBack;

        private Camera cam;

        private float defaultFOV;

        private ProcessHost proc = new ProcessHost();

        private const float PED_WALK_SPEED = 1.4f;

        private ControlSet ctrlZoomIn = new ControlSet(Keys.D7, Keys.None, ControllerButtons.None);
        private ControlSet ctrlZoomOut = new ControlSet(Keys.D8, Keys.None, ControllerButtons.None);

        private string MSG_ZOOM => $"Press {ctrlZoomIn.Description} to zoom-in or {ctrlZoomOut.Description} to zoom-out.";

        public Scene2PedsContact(SceneData sceneData)
        {
            data = sceneData;
        }

        public void Create()
        {
            Spawn();
        }

        public void Start()
        {
            PedTasks();

            proc[ArePlayersTogether] = true;
            proc[DisplayZoomInfo] = true;

            proc.Start();
        }

        private void Spawn()
        {
            var pedStandingData = data.Items.FirstOrDefault(d => d.ID == "pedStand");
            pedStanding = CreateItem(pedStandingData, (m, p, h) => new Ped(m, p, h));
            pedStandingScenario = pedStandingData.Scenario;

            var pedWalkData = data.Items.FirstOrDefault(d => d.ID == "pedWalk");
            pedWalking = CreateItem(pedWalkData, (m, p, h) => new Ped(m, p, h));

            pedWalkDest = pedWalkData.Spawn.Position;

            cam = GenerateItem(data.Items.FirstOrDefault(c => c.ID == "camera")) as Camera;
            cam.PointAtEntity(pedStanding, Vector3.Zero, false);

            defaultFOV = cam.FOV;
            cam.Active = true;
        }

        private void DisplayZoomInfo()
        {
            //TODO:
            //
            // - adjust step (reduce)

            Game.DisplaySubtitle(MSG_ZOOM);

            if(ctrlZoomIn.IsActive)
            {
                for (var i = 0; i < 100; i++)
                {
                    cam.FOV -= cam.FOV > 12 ? 0.1f : 0.05f;
                    GameFiber.Wait(10);
                }
            }

            if(ctrlZoomOut.IsActive)
            {
                if (cam.FOV >= defaultFOV) return;

                for (var i = 0; i < 100; i++)
                {
                    cam.FOV += 0.1f;
                    GameFiber.Wait(10);
                }
            }
        }
        
        private void PedTasks()
        {
            taskGotoPed = pedWalking.Tasks.FollowNavigationMeshToPosition(pedStanding.GetOffsetPositionRight(0.8f), 0, PED_WALK_SPEED);
            NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(pedStanding, pedStandingScenario, 0, true);
        }

        private void ArePlayersTogether()
        {
            if(taskGotoPed.Status == TaskStatus.None)
            {
                var initHeading = pedStanding.Heading;

                NativeFunction.Natives.TaskTurnPedToFaceEntity(pedStanding, pedWalking, 3000);
                NativeFunction.Natives.TaskTurnPedToFaceEntity(pedWalking, pedStanding, 3000);

                GameFiber.Wait(3000);

                //WORLD_HUMAN_DRUG_DEALER

                pedStanding.Tasks.PlayAnimation("missfbi3_party_d", "stand_talk_loop_a_female", 1f, AnimationFlags.Loop);
                pedWalking.Tasks.PlayAnimation("missfbi3_party_d", "stand_talk_loop_a_male1", 1f, AnimationFlags.Loop);

                GameFiber.Wait(10000);

                pedStanding.Tasks.Clear();
                pedStanding.Tasks.AchieveHeading(initHeading);

                pedWalking.Tasks.Clear();

                taskGetBack = pedWalking.Tasks.FollowNavigationMeshToPosition(pedWalkDest, 0, PED_WALK_SPEED);

                GameFiber.Wait(2000);

                NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(pedStanding, pedStandingScenario, 0, true);

                proc.SwapProcesses(ArePlayersTogether, CanFinish);
            }
        }

        private void CanFinish()
        {
            if(taskGetBack.Status == TaskStatus.None)
            {
                if(cam) cam.Delete();

                proc[CanFinish] = false;
                proc[DisplayZoomInfo] = false;

                HasFinished = true;
            }
        }

        public void Dispose()
        {
            if(cam) cam?.Delete();
            proc.Stop();
        }
    }
}
