using Rage;

namespace LSNoir.Stages
{
    interface IMarker
    {
        bool Visible { get; set; }
        void Dispose();
    }
    /// <summary>
    /// Remember to set Visible to true
    /// </summary>
    internal class Marker : IMarker
    {
        public System.Drawing.Color Color { get; set; }
        public bool Visible
        {
            get
            {
                return canRun;
            }
            set
            {
                canRun = value;
            }
        }
        public Vector3 Position { get; set; }
        //public int Alpha { }

        private bool canRun = false;
        private bool loop = true;
        private GameFiber process;

        private enum MarkerTypes
        {
            MarkerTypeUpsideDownCone = 0,
            MarkerTypeVerticalCylinder = 1,
            MarkerTypeThickChevronUp = 2,
            MarkerTypeThinChevronUp = 3,
            MarkerTypeCheckeredFlagRect = 4,
            MarkerTypeCheckeredFlagCircle = 5,
            MarkerTypeVerticleCircle = 6,
            MarkerTypePlaneModel = 7,
            MarkerTypeLostMCDark = 8,
            MarkerTypeLostMCLight = 9,
            MarkerTypeNumber0 = 10,
            MarkerTypeNumber1 = 11,
            MarkerTypeNumber2 = 12,
            MarkerTypeNumber3 = 13,
            MarkerTypeNumber4 = 14,
            MarkerTypeNumber5 = 15,
            MarkerTypeNumber6 = 16,
            MarkerTypeNumber7 = 17,
            MarkerTypeNumber8 = 18,
            MarkerTypeNumber9 = 19,
            MarkerTypeChevronUpx1 = 20,
            MarkerTypeChevronUpx2 = 21,
            MarkerTypeChevronUpx3 = 22,
            MarkerTypeHorizontalCircleFat = 23,
            MarkerTypeReplayIcon = 24,
            MarkerTypeHorizontalCircleSkinny = 25,
            MarkerTypeHorizontalCircleSkinny_Arrow = 26,
            MarkerTypeHorizontalSplitArrowCircle = 27,
            MarkerTypeDebugSphere = 28
        };

        public Marker(Vector3 pos, System.Drawing.Color color)
        {
            Position = pos;
            Color = color;

            process = new GameFiber(Process);
            process.Start();
        }

        public void Dispose()
        {
            loop = false;
            process.Abort();
        }

        private void Process()
        {
            while (loop)
            {
                GameFiber.Yield(); //prior to continue!

                if (!canRun) continue;

                Rage.Native.NativeFunction.CallByName<uint>(
                    "DRAW_MARKER",
                    (int)MarkerTypes.MarkerTypeUpsideDownCone,

                    Position.X,
                    Position.Y,
                    Position.Z,

                    Position.X,
                    Position.Y,
                    Position.Z,

                    0.0f, 0.0f, 0.0f,

                    1.0f, 1.0f, 1.0f,

                    (int)Color.R,
                    (int)Color.G,
                    (int)Color.B,
                    100, //alpha
                    false, true,
                    "", "",
                    false);

            }
        }
    }
}
