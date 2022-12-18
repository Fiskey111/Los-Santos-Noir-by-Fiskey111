namespace LSNoir
{/*
    class PoliceStationCheck
    {
        private static bool _shown, _startedComp;
        private static Marker _marker;

        internal static void PoliceCheck()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    LoadPDComputer();
                    GameFiber.Yield();
                }
            });
        }

        private static void LoadPDComputer()
        {
            var closestLoc = GetClosestLoc();
            if (!(Game.LocalPlayer.Character.Position.DistanceTo(closestLoc) < 20f)) return;
            if (_shown == false)
            {
                _shown = true;
                _marker = new LSNoir.Common.UI.Marker(closestLoc, Color.Yellow);
            }
            _marker?.DrawMarker();
            if (!(Game.LocalPlayer.Character.Position.DistanceTo(closestLoc) < 1.75f)) return;

            Game.DisplayHelp($"Press {Settings.Settings.ComputerKey()} to open the computer");

            if (!Game.IsKeyDown(Settings.Settings.ComputerKey()) || _startedComp) return;

            _startedComp = true;
            Game.IsPaused = true;
            Computer.StartComputerHandler();

            while (Computer.Controller.IsRunning)
                GameFiber.Yield();

            Background.DisableBackground(Background.Type.Computer);
            Computer.AbortController();
            Game.IsPaused = false;
            _startedComp = false;
        }

        private static Vector3 GetClosestLoc()
        {
            var station = new Vector3();
            var stations = new List<Vector3>
            {
                new Vector3(1853, 3690, 34),
                new Vector3(-449, 6012, 32),
                new Vector3(460, -989, 25)
            };
            float closest = 100000f;

            foreach (var sp in stations)
            {
                if (!(sp.DistanceTo(Game.LocalPlayer.Character.Position) < closest)) continue;

                closest = sp.DistanceTo(Game.LocalPlayer.Character.Position);
                station = sp;
            }
            return station;
        }
    }*/
}
