using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneratePlane : MonoBehaviour
{
    [Range(1, 1000)]
    public int tileWidth; //The number of vertices for a single tile
    [Range(1, 500)]
    public int scale; //
    [Range(1, 100)]
    public int mapSize;

    private GameObject container;

    private void Start()
    {
        container = new GameObject();
        container.name = "Generated Mesh";
    }

    public void Resfresh(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            DestroyImmediate(tile);
        }
        tiles.Clear();
    }

    public GameObject createTile(Vector3 pos, int verticesPerMeter)
    {
        print("Creating Tile:");
        GameObject tile = new GameObject();
        tile.transform.position = pos;

        print("\tAdding Ccomponents to tile");
        tile.AddComponent(typeof(MeshFilter));
        tile.AddComponent(typeof(MeshRenderer));
        print("\tSetting Child Parent");
        print(tile);
        tile.transform.parent = container.transform;
        print("\tSetting Material");
        tile.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        createMesh(tile.GetComponent<MeshFilter>(), verticesPerMeter);

        return tile;
    }

    void createMesh(MeshFilter filter, int verticesPerMeter)
    {
        int verticesWidthCount = (tileWidth + 1) * verticesPerMeter;
        Vector3[] arr_positions = new Vector3[(int)Mathf.Pow(verticesWidthCount, 2)];

        int[] triangles = new int[(int)(Mathf.Pow(tileWidth*verticesPerMeter,2) * 6)]; //6 is the amount of vertiuces needed for 2 triangles

        Vector2[] uvs = new Vector2[arr_positions.Length];

        float halfRes = (tileWidth) / 2;


        //Vertex Generation
        for (int c = 0; c < verticesWidthCount; c++)
        {
            for (int r = 0; r < verticesWidthCount; r++)
            {
                Vector3 vert = new Vector3((r/verticesPerMeter)-halfRes, 0, (c/verticesPerMeter)-halfRes);
                int i = (verticesWidthCount * c) + (r);
                arr_positions[i] = vert;
                uvs[i] = new Vector2(r/(float)verticesWidthCount, c/(float)verticesWidthCount);
            }
        }

        //Triangle Calculation
        for (int i=0, c = 0; c < tileWidth*verticesPerMeter; c++)
        {
            for (int r = 0; r < tileWidth*verticesPerMeter; ++r, i+=6)
            {
                int bottomLeft = r + (c * verticesWidthCount);
                int bottomRight = bottomLeft + 1;
                int topLeft = r + ((c + 1) * verticesWidthCount);
                int topRight = topLeft + 1;

                triangles[i] = bottomLeft;
                triangles[i + 1] = topLeft;
                triangles[i + 2] = bottomRight;

                triangles[i + 3] = topLeft;
                triangles[i + 4] = topRight;
                triangles[i + 5] = bottomRight;
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(arr_positions);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateUVDistributionMetrics();

        filter.mesh = mesh;
    }
}
