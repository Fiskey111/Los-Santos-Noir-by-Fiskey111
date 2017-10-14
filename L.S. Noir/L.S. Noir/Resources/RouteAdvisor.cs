using LtFlash.Common.InputHandling;
using Rage;
using System.Drawing;
using System.Windows.Forms;

namespace LSNoir.Resources
{
    class RouteAdvisor
    {
        public Color RouteColor { get; set; } = Color.Yellow;
        public Vector3 Position { get; }
        public float DeactivationDistance { get; set; } = 20f;

        private Blip blip;
        private GameFiber fiber;
        private bool active;
        private bool disableOnClose;

        private readonly ControlSet activate = new ControlSet(Keys.T, Keys.None, ControllerButtons.None);

        public RouteAdvisor(Vector3 pos) => Position = pos;

        public void Start(bool displayHelp = false, bool displayNotification = false, bool deactivateWhenClose = true)
        {
            if (active) return;
            active = true;
            disableOnClose = deactivateWhenClose;

            fiber = GameFiber.StartNew(Process);

            if(displayHelp)
            {
                activate.ColorTag = "~y~";
                Game.DisplayHelp($"Press {activate.Description} to set your GPS.");
            }
            if(displayNotification)
            {
                activate.ColorTag = "~y~";
                Game.DisplayNotification($"Press {activate.Description} to set your GPS.");
            }
        }

        public void Stop()
        {
            if (blip) blip.Delete();
            active = false;
        }

        private void Process()
        {
            while(active)
            {
                GameFiber.Yield();

                if(activate.IsActive)
                {
                    if (!blip)
                    {
                        blip = new Blip(Position)
                        {
                            RouteColor = RouteColor,
                            IsRouteEnabled = true,
                            Scale = 0,
                        };
                    }
                    else blip.Delete();
                }

                if(disableOnClose && blip)
                {
                    if(Vector3.Distance(Position, Game.LocalPlayer.Character.Position) < DeactivationDistance)
                    {
                        Stop();
                    }
                }
            }
        }

        ~RouteAdvisor()
        {
            if (blip) blip.Delete();
        }
    }
}
