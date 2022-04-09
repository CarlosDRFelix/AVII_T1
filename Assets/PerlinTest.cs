using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTest : MonoBehaviour
{
    float t = 0;
    float tt1, tt2, tt3 = 0;
    public float inc1 = 0.01f;
    public float inc2 = 0.05f;
    public float inc3 = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        /*float hc1 = 0.5F * (Mathf.Cos(2 * t+0.4f) + 1);
        float hc2 = 0.25F * (Mathf.Cos(4 * t) + 1);
        float hc3 = 0.125F * (Mathf.Cos(6 * t) + 1);
        float hr = Random.Range(0f, 1f);*/

        t += Time.deltaTime;
        tt1 += inc1;
        tt2 += inc2;
        tt3 += inc3;

        float hp1 = Mathf.PerlinNoise(tt1,1);
        float hp2 = 0.25f * Mathf.PerlinNoise(tt2, 1);
        float hp3 = 0.125f * Mathf.PerlinNoise(tt3, 1);

        /*Grapher.Log(hc1, "Cos", Color.green);
        Grapher.Log(hr, "Random", Color.red);
        Grapher.Log(hc1 + hc2 + hc3, "CosSum", Color.cyan);*/

        Grapher.Log(hp1+hp2+hp3, "Perlin", Color.yellow );
    }
}
