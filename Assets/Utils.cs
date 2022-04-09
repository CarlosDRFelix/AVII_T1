using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    static float smooth = 0.01f;
    static int maxHeight = 50;
    static int octaves = 6;
    static float persistence = 0.7f;


    public static int GenerateHeight(float x, float z)
    {
        return (int)Map(0, maxHeight, 0, 1, fBM(x * smooth, z * smooth,
            octaves, persistence));
    }

    public static int GenerateStoneHeight(float x, float z)
    {
        return (int)Map(0, maxHeight-10, 0, 1, fBM(x * smooth * 2, z * smooth* 2,
            octaves-1, 1.2f*persistence));
    }

    static float Map(float newmin, float newmax, float orimin, float orimax, float val)
    {
        return Mathf.Lerp(newmin,newmax, Mathf.InverseLerp(orimin, orimax, val));
    }

    public static float fBM3d(float x, float y, float z, int octaves, float persistence)
    {
        float xy = fBM(x * smooth, y * smooth, octaves, persistence);
        float yx = fBM(y * smooth, x * smooth, octaves, persistence);
        float xz = fBM(x * smooth, z * smooth, octaves, persistence);
        float zx = fBM(z * smooth, x * smooth, octaves, persistence);
        float zy = fBM(z * smooth, y * smooth, octaves, persistence);
        float yz = fBM(y * smooth, z * smooth, octaves, persistence);

        return (xy + yx + xz + zx + yz + zy) / 6;
    }


    static float fBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;

        for (int i=0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x*frequency, z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
            
        }
        return total / maxValue;
    }

}
