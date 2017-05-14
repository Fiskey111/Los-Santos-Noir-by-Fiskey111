using LSNoir.Data;
using LtFlash.Common.InputHandling;
using Rage;
using Rage.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSNoir.Computer
{
    class ComputerController
    {
        //http://gtaxscripting.blogspot.com/2016/05/gta-v-blips-id-and-image.html

        //TODO:
        // - wnd list passed by ctor param -> check if All(closed)
        //   to switch state to isActive = false;
        // - use Checkpoint to mark the closest computer - don't make one for all!
        // - load computer positions from xml to easily update the mod via internet

        private readonly Vector3[] positions =
        {
            new Vector3(-2.349341f, 533.5342f, 175.3423f),
        };

        private readonly List<Blip> blips = new List<Blip>();

        private GameFiber fiber;
        private bool canRun;
        private bool isComputerActive;
        private const float DIST_ACTIVE = 5.5f;
        private readonly ControlSet controlSet = new ControlSet(Keys.E, Keys.LControlKey, ControllerButtons.None);
        private MainForm wnd;
        private readonly List<GwenForm> wnds = new List<GwenForm>();
        private List<CaseData> getCaseData;
        //private Checkpoint

        public ComputerController(List<CaseData> getCaseData)
        {
            this.getCaseData = getCaseData;
            controlSet.ColorTag = "~y~";
        }

        public void Start()
        {
            canRun = true;
            fiber = GameFiber.StartNew(Process);

            for (int i = 0; i < positions.Length; i++)
            {
                var b = new Blip(positions[i]);
                b.Sprite = (BlipSprite)407;
                b.Color = System.Drawing.Color.Blue;
                blips.Add(b);
            }
        }

        public void Stop()
        {
            canRun = false;
            blips.ForEach(b => { if (b) b.Delete(); });
            blips.Clear();
        }

        public void Process()
        {
            while (canRun)
            {
                GameFiber.Yield();

                if (!isComputerActive && IsAnyWithinDist())
                {
                    Game.DisplaySubtitle($"Press {controlSet.Description} to open terminal.", 100);

                    if(controlSet.IsActive)
                    {
                        GameFiber.StartNew(() =>
                        {
                            wnd = new MainForm(getCaseData);
                            wnd.Show();
                            isComputerActive = true;
                        });

                        Game.LocalPlayer.HasControl = false;
                    }
                }
                else if(isComputerActive && wnds.All(w => !w.Window.IsVisible))
                {
                    isComputerActive = false;
                    Game.LocalPlayer.HasControl = true;
                }
            }
        }
        //TODO: return active to create a checkpoint
        private bool IsAnyWithinDist() => positions.Any(p => IsWithinDist(p));

        private bool IsWithinDist(Vector3 p)
            => Vector3.Distance(p, Game.LocalPlayer.Character.Position) < DIST_ACTIVE;

        ~ComputerController()
        {
            Game.LocalPlayer.HasControl = true;
            Stop();
        }
    }
}
