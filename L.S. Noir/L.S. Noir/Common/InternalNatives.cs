using static Rage.Native.NativeFunction;

namespace LSNoir.Common
{
    public static class InternalNatives
    {
        public static void GetActiveScreenResolution(out int x, out int y) => Natives.x873C9F3104101DD3(out x, out y);
    }
}