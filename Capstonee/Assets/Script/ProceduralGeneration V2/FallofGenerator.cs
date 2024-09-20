
using UnityEngine;

public static class FallofGenerator
{
    public static float FallofSize(float i, float size) => i/(float)size * 2 - 1;
    public static float[,] GenerateFallofMap(int size, float FalloffOffset, float FalloffMultiplier)
    {
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++){
                float x = FallofSize(i, size);
                float y = FallofSize(j, size);

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i,j] = Evaluate(value, FalloffOffset, FalloffMultiplier);
            }
        }
        return map;
    }
    static float Evaluate(float value, float Offset, float Multiplier){
        float a = Multiplier;
        float b = Offset;
        return Mathf.Pow(value, 2) / Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a);
    }
}
