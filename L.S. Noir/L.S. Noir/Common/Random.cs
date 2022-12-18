namespace LSNoir.Common
{
    public class Random
    {
        public static System.Random RandomGenerator = new System.Random();

        public static int RandomInt(int max) => max <= 0 ? 0 : RandomGenerator.Next(max);
        public static int RandomInt(int min, int max) => max <= min + 1 ? 0 : RandomGenerator.Next(min, max);

        public static float RandomFloat(float max)
        {
            var sign = RandomGenerator.Next(2);
            var exponent = RandomGenerator.Next((1 << 8) - 1); // do not generate 0xFF (infinities and NaN)
            var mantissa = RandomGenerator.Next(1 << 23);

            var bits = (sign << 31) + (exponent << 23) + mantissa;
            return IntBitsToFloat(bits);
        }

        private static float IntBitsToFloat(int bits)
        {
            unsafe
            {
                return *(float*) &bits;
            }

        }
    }
}