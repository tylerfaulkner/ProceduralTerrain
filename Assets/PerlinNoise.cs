/*************************************************
 * Perlin Noise for Procedural Generated Terrain
 * 
 * By: Tyler Faulkner
 * 
 * Created: 11/24/202
 * 
 * **********************************************
 */

 /*
  * --Imports--
  */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * --Perlin Noise Class--
 * 
 * Generates and applies a perlin noise function
 * to a plane created through Generate Plane Script
 * 
 * Has basic functionality to translate perlin noise
 * to a terrain interpretation
 */
[RequireComponent(typeof(GeneratePlane))]
public class PerlinNoise : MonoBehaviour
{

    [SerializeField]
    bool useTerrainColors = false;

    [SerializeField]
    bool applyHeight = false; //False, only adds texture does not alter vertices pos

    [SerializeField]
    public float lacunarity;

    [SerializeField]
    public float height;

    [Range(1,10)]
    public int octaves;

    [Range(0,1)]
    public float persistance; //alters effect of amplitude on next octave, higher lower influence

    [Range(1,300)]
    public float zoom = 1;

    public AnimationCurve heightCurve;

    public int seed = 104;
    private Vector2[] octaveOffsets;

    public Color peakColor;
    public float peakLowerBound;

    public float shiftZ = -27;
    public float shiftX = -27;

    public bool autogenerate = false;

    private GeneratePlane generator;

    public HeightColor[] colors;

    [Range(0, 1)]
    public float minMax = 0.7f;


    [System.Serializable]
    public struct HeightColor
    {
        public string name;
        [Range(0,1)]
        public float height;
        public Color color;

        public HeightColor(string name, float height, Color color)
        {
            this.name = name;
            this.height = height;
            this.color = color;
        }
    }

    private void Start()
    {
        generator = GetComponent<GeneratePlane>();
        octaveOffsets = new Vector2[octaves];
        System.Random rng = new System.Random(seed);
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = (float)rng.Next(-100000, 100000) + shiftX;
            float offsetY = (float)rng.Next(-100000, 100000) + shiftZ;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
    }

    private void OnValidate()
    {
        if (autogenerate && Application.isPlaying)
        {
            //generateTerrain();
            System.Random rng = new System.Random(seed);
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = (float)rng.Next(-100000, 100000) + shiftX;
                float offsetY = (float)rng.Next(-100000, 100000) + shiftZ;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
        }
    }

    [ContextMenu("Default")]
    void defaultSettings()
    {
        lacunarity = 2;
        octaves = 4;
        persistance = 0.5f;
        shiftX = 0;
        shiftZ = 0;
        zoom = 2;
        applyHeight = false;
        useTerrainColors = false;
    }

    [ContextMenu("Default - Terrain")]
    void defaultTerrain()
    {
        defaultSettings();
        applyHeight = true;
        height = 75;
        useTerrainColors = true;
        colors = new HeightColor[8];
        colors[0] = new HeightColor("water", 0.2f, new Color(0,0.6f,0.9f));
        colors[1] = new HeightColor("shallow water", 0.25f, new Color(0, 0.65f, 1));
        colors[2] = new HeightColor("sand", 0.29f, Color.yellow);
        colors[3] = new HeightColor("grass", 0.6f, Color.green);
        colors[4] = new HeightColor("forest", 0.65f, new Color(0, 0.75f, 0));
        colors[5] = new HeightColor("Lower Mountain", 0.75f, new Color(0.4f, 0.4f, 0.4f));
        colors[6] = new HeightColor("Mountain", 0.9f, Color.grey);
        colors[7] = new HeightColor("Snow", 1, Color.white);
    }

    public (float, float) perlinMesh(MeshFilter filter)
    {
        Mesh m = filter.mesh;

        Vector3[] vertices = m.vertices;

        int width = generator.tileWidth + 1;

        float current_persistance = persistance;
        if (persistance == 0)
        {
            current_persistance = 0.001f;
        }


        float maxAmplitude = float.MinValue;
        float minAmplitude = float.MaxValue;

        float xOrg = filter.transform.position.x/generator.tileWidth * generator.tileWidth;
        float zOrg = filter.transform.position.z/generator.tileWidth * generator.tileWidth;

        
        //calculates perlin of every point
        for (int i = 0; i < vertices.Length; i++)
        {

            Vector3 vert = vertices[i];
            float x = vert.x;
            float z = vert.z;
            float y = 0;

            //Dry run:
            //start at 1 frequency and amplitude
            //increase from there
            float currentFrequency = 1;
            float currentAmplitude = 1;

            float tileArea = width * width;

            for (int o=0; o < octaves; o++)
            {

                float a = (xOrg + x) / zoom;
                float b = (zOrg + z) / zoom;


                float xVal = (((a) * currentFrequency)) + octaveOffsets[o].x;
                float zVal = (((b) * currentFrequency)) + octaveOffsets[o].y;

                float perlPoint = Mathf.PerlinNoise(xVal, zVal) * 2 - 1;

                y += perlPoint * currentAmplitude;

                currentFrequency *= lacunarity;
                currentAmplitude *= persistance;
            }

            if (y > maxAmplitude)
            {
                maxAmplitude = y;
            }
            if (y < minAmplitude){

                minAmplitude = y;
            }

            vertices[i] = new Vector3(x, y, z);


        }


        m.vertices = vertices;

        filter.mesh = m;

        return (maxAmplitude, minAmplitude);
    }

    public void addColors(float minAmplitude, float maxAmplitude, MeshFilter filter, float height, int verticesPerMeter)
    {
        Mesh m = filter.mesh;

        Vector3[] vertices = m.vertices;

        Color[] colorMap = new Color[vertices.Length];


        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = vertices[i];

            float y = vert.y;

            vert.y = Mathf.InverseLerp(minAmplitude, maxAmplitude, vert.y);


            if (applyHeight)
            {
                vertices[i] = new Vector3(vert.x, heightCurve.Evaluate(vert.y) * height, vert.z);
            }
            else
            {
                vertices[i] = new Vector3(vert.x, 0, vert.z);
            }

            if (useTerrainColors)
            {
                colorMap[i] = getColor(vert.y);
            }
            else
            {
                colorMap[i] = Color.Lerp(Color.black, Color.white, vert.y);
            }
        }

        int width = (generator.tileWidth + 1)*verticesPerMeter;

        Texture2D text = new Texture2D(width, width, TextureFormat.ARGB32, false);
        text.filterMode = FilterMode.Trilinear;
        text.wrapMode = TextureWrapMode.Clamp;
        MeshRenderer renderer = filter.transform.GetComponent<MeshRenderer>();
        Material mat = renderer.sharedMaterial;
        mat.SetTexture("_MainTex", text);

        m.vertices = vertices;
        m.RecalculateBounds();

        text.SetPixels(colorMap);
        text.Apply();
    }



    /*
     * Color Generator
     * 
     * DEPRECATED
     */
    private Color getMapColor(float heightScale)
    {

        if (heightScale >= 0.85)
        {
            return peakColor;
        }
        else if (heightScale >= 0.7)
        {
            return Color.grey;
        }
        else if (heightScale >= 0.45)
        {
            return Color.green;
        }
        else if (heightScale >= 0.4)
        {
            return Color.yellow;
        }
        else if (heightScale >= 0.15)
        {
            return Color.cyan;
        }
        else
        {
            return Color.blue;
        }
    }

    private Color getColor(float y)
    {
        foreach (HeightColor heightColor in colors)
        {
            if (y <= heightColor.height)
            {
                return heightColor.color;
            }
        }
        return Color.clear;
    }
}
