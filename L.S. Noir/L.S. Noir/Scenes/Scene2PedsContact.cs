using LSNoir.Data;
using LtFlash.Common.InputHandling;
using LtFlash.Common.Processes;
using Rage;
using Rage.Native;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Scenes
{
    interface ISceneActive : IScene
    {
        bool HasFinished { get; }
    }

    class Scene2PedsContact : SceneBase, ISceneActive
    {
        public bool HasFinished { get; private set; }

        private readonly SceneData data;
        private Ped pedWalking;
        private Vector3 pedWalkDest;
        private Ped pedStanding;

        private Task taskGotoPed;
        private Task taskGetBack;

        private Camera cam;

        private float defaultFOV;
        private float minFOV = 8f;

        private ProcessHost proc = new ProcessHost();

        private const float PED_WALK_SPEED = 1.5f;

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
            PedTasks();

            proc[ArePlayersTogether] = true;
            proc[DisplayZoomInfo] = true;

            proc.Start();
        }

        private void Spawn()
        {
            var pedStandingData = data.Items.FirstOrDefault(d => d.ID == "pedStand");
            pedStanding = CreateItem(pedStandingData, (m, p, h) => new Ped(m, p, h));

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
                for (var i = 0; i < 10; i++)
                {
                    cam.FOV -= 2f;
                    GameFiber.Wait(100);
                }

                Game.LogTrivial("Curr FOV: " + cam.FOV);
            }

            if(ctrlZoomOut.IsActive)
            {
                for (var i = 0; i < 10; i++)
                {
                    cam.FOV += 2f;
                    GameFiber.Wait(100);
                }

                Game.LogTrivial("Curr FOV: " + cam.FOV);
            }
        }
        
        private void PedTasks()
        {
            taskGotoPed = pedWalking.Tasks.FollowNavigationMeshToPosition(pedStanding.GetOffsetPositionRight(2.0f), 0, PED_WALK_SPEED);
        }

        private void ArePlayersTogether()
        {
            if(taskGotoPed.Status == TaskStatus.None)
            {
                NativeFunction.Natives.TaskTurnPedToFaceEntity(pedStanding, pedWalking, 3000);
                NativeFunction.Natives.TaskTurnPedToFaceEntity(pedWalking, pedStanding, 3000);

                // play anim talk
                //WORLD_HUMAN_DRUG_DEALER
                GameFiber.Wait(10000);

                taskGetBack = pedWalking.Tasks.FollowNavigationMeshToPosition(pedWalkDest, 0, PED_WALK_SPEED);

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
