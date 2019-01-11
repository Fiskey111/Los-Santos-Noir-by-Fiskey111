using LSNoir.Data;
using LSNoir.Settings;
using Rage;
using Rage.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
        // - references of Gwen.Controls can't be placed in ctor, otherwise it'll throw a NullRef ex

        private readonly Vector3[] positions;

        private readonly List<Blip> blips = new List<Blip>();

        private GameFiber fiber;
        private bool canRun;
        private bool isComputerActive;

        private const float DIST_ACTIVE = 1f;
        private const string MSG_PRESS_TO_OPEN = "Press {0} to open terminal.";

        private const BlipSprite BLIP_SPRITE_COMPUTER = (BlipSprite)407;
        private readonly Color COLOR_BLIP_COMPUTER = Color.Blue;

        private readonly int GameResWidth = Game.Resolution.Width;
        private readonly int GameResHeight = Game.Resolution.Height;

        private readonly ControlSet controlSet = Main.Controls.ActivateComputer;

        private readonly List<GwenForm> wnds = new List<GwenForm>();
        private readonly Func<List<CaseData>> activeCasesData;
        //private Checkpoint
        private Texture computerBackground;

        public ComputerController(Func<List<CaseData>> getCaseData)
        {
            activeCasesData = getCaseData;

            controlSet.ColorLetter = "y";

            if (File.Exists(Paths.PATH_COMPUTER_POSITIONS))
            {
                positions = DataAccess.DataProvider.Instance.Load<List<Vector3>>(Paths.PATH_COMPUTER_POSITIONS).ToArray();
            }
            else
            {
                var msg = $"{nameof(ComputerController)}(): file with computer positions could not be found: {Paths.PATH_COMPUTER_POSITIONS}";
                throw new FileNotFoundException(msg);
            }
        }

        public void Start()
        {
            canRun = true;
            fiber = GameFiber.StartNew(Process);

            var pb = positions.Select(p => CreateBlip(p, BLIP_SPRITE_COMPUTER, COLOR_BLIP_COMPUTER));
            blips.AddRange(pb);

            computerBackground = LoadBackground(Paths.PATH_COMPUTER_BACKGROUND);

            Game.RawFrameRender += RawRender;
        }

        private static Texture LoadBackground(string path)
        {
            if (!File.Exists(Paths.PATH_COMPUTER_BACKGROUND))
            {
                string msg = $"{nameof(ComputerController)}.{nameof(Start)}(): computer background was not found: {Paths.PATH_COMPUTER_BACKGROUND}";
                throw new FileNotFoundException(msg);
            }

            return Game.CreateTextureFromFile(path);
        }
        
        private static Blip CreateBlip(Vector3 pos, BlipSprite sprite, Color col)
        {
            return new Blip(pos)
            {
                Sprite = sprite,
                Color = col,
                Scale = 0.75f,
            //TODO: DisplayType: no-minimap, map only
            };
        }

        private void RawRender(object sender, GraphicsEventArgs e)
        {
            if(isComputerActive)
            {
                e.Graphics.DrawTexture(computerBackground, 0f, 0f, GameResWidth, GameResHeight);
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
                    Game.DisplaySubtitle(string.Format(MSG_PRESS_TO_OPEN, controlSet.GetDescription()), 100);

                    if(controlSet.IsActive())
                    {
                        DisplayComputer();
                    }
                }
                else if(isComputerActive && wnds.All(w => !w.Window.IsVisible))
                {
                    CloseComputer();
                }
            }
        }

        private void DisplayComputer()
        {
            GameFiber.StartNew(() =>
            {
                var mainWnd = new MainForm(this, activeCasesData());
                AddWnd(mainWnd);
                mainWnd.Show();

                isComputerActive = true;
            });

            Game.LocalPlayer.HasControl = false;
        }

        private void CloseComputer()
        {
            isComputerActive = false;

            Game.LocalPlayer.HasControl = true;
        }

        //TODO: return active to create a checkpoint
        private bool IsAnyWithinDist() => positions.Any(p => IsWithinDist(p));

        private bool IsWithinDist(Vector3 p)
            => Vector3.Distance(p, Game.LocalPlayer.Character.Position) < DIST_ACTIVE;

        private static Vector3 GetPositionInRange(Vector3[] pos, Vector3 playerPos, float range)
            => pos.FirstOrDefault(p => Vector3.Distance(p, playerPos) < range);

        ~ComputerController()
        {
            Game.LocalPlayer.HasControl = true;
            Stop();
        }
    }
}
