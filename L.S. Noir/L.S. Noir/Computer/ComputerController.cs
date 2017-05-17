using LSNoir.Data;
using LSNoir.Settings;
using LtFlash.Common.InputHandling;
using LtFlash.Common.Serialization;
using Rage;
using Rage.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Computer
{
    class ComputerController
    {
        //http://gtaxscripting.blogspot.com/2016/05/gta-v-blips-id-and-image.html

        //TODO:
        // - use Checkpoint to mark the closest computer - don't make one for all!

        //NOTES:
        // - wnd list passed by ctor param -> check if All(closed)
        //   to switch state to isActive = false;
        // - add empty lines to TextBox.Multiline = true in WinForm or it'll throw 
        //   an exception while using SetTextLine on Gwen.MultilineTextBox
        // - wnd size: 800x600, left-side listBox: h200; buttons: 100x100

        private readonly Vector3[] positions;

        private readonly List<Blip> blips = new List<Blip>();

        private GameFiber fiber;
        private bool canRun;
        private bool isComputerActive;
        private const float DIST_ACTIVE = 1f;

        private readonly ControlSet controlSet = new ControlSet(Controls.KeyActivateComputer, Controls.ModifierActivateComputer, Controls.CtrlButtonActivateComputer);

        private readonly List<GwenForm> wnds = new List<GwenForm>();
        private readonly List<CaseData> activeCasesData;
        //private Checkpoint
        private Texture computerBackground;

        public ComputerController(List<CaseData> getCaseData)
        {
            activeCasesData = getCaseData;
            controlSet.ColorTag = "~y~";

            if (File.Exists(Paths.PATH_COMPUTER_POSITIONS))
            {
                positions = Serializer.LoadFromXML<Vector3>(Paths.PATH_COMPUTER_POSITIONS).ToArray();
            }
            else
            {
                var msg = $"{nameof(ComputerController)}.{nameof(ComputerController)}(): file with computer positions could not be found: {Paths.PATH_COMPUTER_POSITIONS}";
                throw new FileNotFoundException(msg);
            }
        }

        public void Start()
        {
            canRun = true;
            fiber = GameFiber.StartNew(Process);

            blips.AddRange(CreateBlipsForPositions(positions, (BlipSprite)407, Color.Blue));

            if (File.Exists(Paths.PATH_COMPUTER_BACKGROUND))
            {
                computerBackground = Game.CreateTextureFromFile(Settings.Paths.PATH_COMPUTER_BACKGROUND);
                Game.RawFrameRender += RawRender;
            }
            else
            {
                string msg = $"{nameof(ComputerController)}.{nameof(Start)}(): computer background was not found: {Settings.Paths.PATH_COMPUTER_BACKGROUND}";
                throw new FileNotFoundException(msg);
            }
        }

        private IEnumerable<Blip> CreateBlipsForPositions(Vector3[] pos, BlipSprite sprite, Color col)
        {
            for (int i = 0; i < pos.Length; i++)
            {
                var b = new Blip(pos[i]);
                b.Sprite = sprite;
                b.Color = col;
                //DisplayType: no-minimap, map only
                yield return b;
            }
        }

        private void RawRender(object sender, GraphicsEventArgs e)
        {
            if(isComputerActive)
            {
                e.Graphics.DrawTexture(computerBackground, 0f, 0f, Game.Resolution.Width, Game.Resolution.Height);
            }
        }

        public void Stop()
        {
            canRun = false;
            wnds.ForEach(w => w.Window.Hide());
            blips.ForEach(b => { if (b) b.Delete(); });
            blips.Clear();
            isComputerActive = false;
            Game.LocalPlayer.HasControl = true;
        }

        public void AddWnd(GwenForm wnd)
        {
            wnds.Add(wnd);
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
                            var mainWnd = new MainForm(this, activeCasesData);
                            AddWnd(mainWnd);
                            mainWnd.Show();

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
