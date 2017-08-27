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

        private Blip blip;
        private GameFiber fiber;
        private bool active;

        private readonly ControlSet activate = new ControlSet(Keys.T, Keys.None, ControllerButtons.None);

        public RouteAdvisor(Vector3 pos) => Position = pos;

        public void Start(bool displayHelp = false, bool displayNotification = false)
        {
            if (active) return;
            active = true;
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

        public void Stop() => active = false;

        private void Process()
        {
            while(active)
            {
                GameFiber.Yield();

                //if (blip) blip.EnableRoute(RouteColor);
                
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
            }
        }

        ~RouteAdvisor()
        {
            if (blip) blip.Delete();
        }
    }
}
