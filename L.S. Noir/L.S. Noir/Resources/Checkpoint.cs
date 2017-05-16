using Rage;
using Rage.Native;
using System.Drawing;

namespace LSNoir.Resources
{
    public static class CheckptExt
    {
        public static bool Exists(this Checkpoint ch) => ch != null && ch.Valid();
    }

    public class Checkpoint
    {
        //TODO:
        // - add bool Visible?

        public Blip Blip
        {
            get => blip;
            set => blip = value;
        }

        private uint handle;
        private float radius;
        private Vector3 position;
        private bool exists = false;
        private Blip blip;

        private const int DEFAULT_TYPE = 5;

        public Checkpoint(Vector3 position, float radius, Color color) : this(position, radius, color, DEFAULT_TYPE)
        {
        }

        public Checkpoint(Vector3 position, Color color) : this(position, 10, color)
        {
        }

        public Checkpoint(Vector3 position, float radius, Color Color, int Type)
        {
            this.radius = radius;
            this.position = position;

            handle = NativeFunction.Natives.CreateCheckpoint(
                        Type,
                        position.X, position.Y, position.Z /*- 1.0f/*7.0f*/,
                        //Position.X, Position.Y, Position.Z - 10.0f,
                        Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z,
                        this.radius, (int)Color.R, (int)Color.G, (int)Color.B,
                        100, 0);

            exists = true;
        }

        public Checkpoint(Vector3 Position, Color Color, bool CreateBlip) : this(Position, Color)
        {
            if (CreateBlip)
            {
                blip = new Blip(Position);
                blip.Color = Color;
            }
        }

        public bool IsPositionInRange(Vector3 Position)
        {
            return Vector3.Distance(Position, position) <= radius;
        }

        public bool Valid() => exists;

        public void Delete()
        {
            Rage.Native.NativeFunction.Natives.DeleteCheckpoint(handle);

            if (blip.Exists()) blip.Delete();

            exists = false;
        }
    }
}
