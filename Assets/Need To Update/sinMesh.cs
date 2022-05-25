using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratePlane))]
public class sinMesh : MonoBehaviour
{
    [SerializeField]
    bool generateSin = false;

    [SerializeField]
    float sinWidth = 1f;

    [SerializeField]
    float sinHeight = 1f;

    [SerializeField]
    bool generateSinSqrt = false;

    [SerializeField]
    float timeMultiplier = 2;

    [Range(0, 500)]
    public int sinSqrtCenter = 0;

    private float totalTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime*timeMultiplier;
        if (generateSin)
        {
            Mesh m = GetComponent<MeshFilter>().mesh;
            Vector3[] points = new Mesh().vertices;//GetComponent<GeneratePlane>().getGeneratedMesh().vertices;
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 point = points[i];
                points[i] = new Vector3(point.x, Mathf.Sin((point.x + point.z + totalTime) / sinWidth) * sinHeight, point.z);
            }
            m.SetVertices(points);
            m.RecalculateNormals();
            m.RecalculateBounds();
        }

        if (generateSinSqrt)
        {
            Mesh m = GetComponent<MeshFilter>().mesh;
            Vector3[] points = new Mesh().vertices;//GetComponent<GeneratePlane>().getGeneratedMesh().vertices;
            for(int i = 0; i < points.Length; ++i)
            {
                Vector3 point = points[i];
                points[i] = new Vector3(point.x, -Mathf.Sin(Mathf.Sqrt(Mathf.Pow(point.x - sinSqrtCenter, 2) + Mathf.Pow(point.z - sinSqrtCenter, 2) + Mathf.Pow(totalTime,2))/sinWidth)*sinHeight, point.z);
            }
            m.SetVertices(points);
            m.RecalculateNormals();
            m.RecalculateBounds();            
        }
    }
}
