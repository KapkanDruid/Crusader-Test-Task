using UnityEngine;

namespace Game.CMS.Runtime.Utils.Helpers
{
    public static class ColorHelper
    {
        public static Color IntToBrightColor(int value)
        {
            uint hash = Hash((uint)value);
            float hue = hash / (float)uint.MaxValue;
            return Color.HSVToRGB(hue, 0.85f, 1f);
        }

        private static uint Hash(uint x)
        {
            x ^= x >> 16;
            x *= 0x7feb352d;
            x ^= x >> 15;
            x *= 0x846ca68b;
            x ^= x >> 16;
            return x;
        }
    }
}
