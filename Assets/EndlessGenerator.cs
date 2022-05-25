using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratePlane))]
[RequireComponent(typeof(PerlinNoise))]
public class EndlessGenerator : MonoBehaviour
{

    private GameObject player;
    private GeneratePlane generator;
    private PerlinNoise perlin;

    private Dictionary<Vector2, GameObject> posToTile;

    private List<GameObject> ActiveTiles;

    [SerializeField]
    private float maxHeight = 150;
    [SerializeField]
    private float minHeight = -150;
    [Range(1,20)]
    public int halfRadius = 2;
    [Range(1,20)]
    public int verticesPerMeter = 1;

    private int currentTileNumX;
    private int currentTileNumZ;

    public bool active = false;

    private float tileRes;
    private float halfRes;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        generator = GetComponent<GeneratePlane>();
        perlin = GetComponent<PerlinNoise>();
        posToTile = new Dictionary<Vector2, GameObject>();
        ActiveTiles = new List<GameObject>();
        tileRes = generator.tileWidth;
        halfRes = tileRes / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Vector3 pos = player.transform.position;
            float tileRes = generator.tileWidth;

            float height = tileRes * 0.8f;
            pos.y = 0;
            //Calculate Player position relative to spawn  (0,0)
            float playerPosX = pos.x > 0 ? pos.x + halfRes : pos.x - halfRes;
            float playerPosZ = pos.z > 0 ? pos.z + halfRes : pos.z - halfRes;

            //Convert Player Pos to tile coordinates
            int tileNumX = (int)playerPosX / generator.tileWidth;
            int tileNumZ = (int)playerPosZ / generator.tileWidth;

            //Check if player moved to a new tile
            if (tileNumX != currentTileNumX || tileNumZ != currentTileNumZ || posToTile.Count == 0)
            {
                //Convert to vector2
                Vector2 gridPos = new Vector2(tileNumX, tileNumZ);
                print(gridPos);

                //Set current Position
                currentTileNumX = tileNumX;
                currentTileNumZ = tileNumZ;

                hideFarTiles(tileNumX, tileNumZ);


                //Check if the tile has already been generated
                inttializeTiles(tileNumX, tileNumZ);

            }
        }
    }

    void inttializeTiles(int startX, int startZ)
    {

        for (int r =startZ-halfRadius; r<=startZ+halfRadius; r++)
        {
            for (int c = startX-halfRadius; c<=startX+halfRadius; c++)
            {
                Vector2 gridPos = new Vector2(c, r);
                if (!posToTile.ContainsKey(gridPos))
                {
                    Vector3 newTilePos = convertGridToPosCords(c, r);
                    GameObject newTile = generator.createTile(newTilePos, verticesPerMeter);

                    //Apply Random Generation and colors
                    perlin.perlinMesh(newTile.GetComponent<MeshFilter>());
                    perlin.addColors(minHeight, maxHeight, newTile.GetComponent<MeshFilter>(), tileRes * 0.65f, verticesPerMeter);

                    posToTile.Add(gridPos, newTile);
                }

                if (!ActiveTiles.Contains(posToTile[gridPos]))
                {
                    posToTile[gridPos].SetActive(true);
                    ActiveTiles.Add(posToTile[gridPos]);
                }
            }
        }
    }

    Vector3 convertGridToPosCords(int gridX, int gridY)
    {
        return new Vector3(gridX * tileRes, 0, gridY * tileRes);
    }

    (int, int) convertPosToGridCords(Vector3 pos)
    {
        return ((int) (pos.x/tileRes), (int) (pos.z/tileRes));
    }

    void hideFarTiles(int playerTileX, int playerTileZ)
    {
        List<int> toRemove = new List<int>();

        float maxZ = playerTileZ + halfRadius;
        float minZ = playerTileZ - halfRadius;

        float maxX = playerTileX + halfRadius;
        float minX = playerTileX - halfRadius;

        for(int i =ActiveTiles.Count-1; i>=0; i--)
        {
            GameObject tile = ActiveTiles[i];

            int zPos;
            int xPos;

            (xPos, zPos) = convertPosToGridCords(tile.transform.position);


            bool zCheck = zPos > maxZ || zPos < minZ;
            bool xCheck = xPos > maxX || xPos < minX;

            if(zCheck || xCheck)
            {
                tile.SetActive(false);
                ActiveTiles.RemoveAt(i);
            }
        }

        foreach (int i in toRemove)
        {
            GameObject tile = ActiveTiles[i];
            tile.SetActive(false);
            ActiveTiles.RemoveAt(i);
        }
        ActiveTiles.TrimExcess();
    }
}
