using System.Numerics;

namespace LD51.Framework;

public static class Rand
{
    private static Random s_random = new();

    public static float Next() => (float)s_random.NextDouble();
    public static float Next(float min, float max) => min + (max - min) * (float)s_random.NextDouble();
}

public static class MathExt
{
    public static float Clamp01(float value)
    {
        if (value < 0f) return 0f;
        if (value > 1f) return 1f;
        return value;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
}