using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeVertices : MonoBehaviour
{
    [SerializeField]
    bool generate = false;
    [SerializeField]
    bool randomY = false;
    [SerializeField]
    bool randomX = false;
    [SerializeField]
    bool randomZ = false;


    // Update is called once per frame
    void Update()
    {
        if (generate)
        {
            Vector3[] startPoints = new Mesh().vertices;

            Mesh m = GetComponent<MeshFilter>().mesh;

            Vector3[] points = new Vector3[startPoints.Length];

            startPoints.CopyTo(points, 0);

            for(int i=0;i<points.Length; i++)
            {
                points[i] += generateRandomVector3();
            }

            m.vertices = points;
            m.RecalculateNormals();

            generate = false;
        }
    }

    Vector3 generateRandomVector3()
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (randomX)
        {
            x = Random.Range(-1f, 1f);
        }

        if (randomY)
        {
            y = Random.Range(-1f, 1f);
        }

        if (randomZ)
        {
            z = Random.Range(-1f, 1f);
        }

        return new Vector3(x, y, z);
    }
}
