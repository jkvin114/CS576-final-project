using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public enum TerrainType
{
    PLAIN = 0,
    ROCKY = 1,
    FOREST = 2,
    MOUNTAIN = 3,
    ROCKY_MOUNTAIN = 4,
}

public class Level : MonoBehaviour
{
    public GameObject plain_terrain;
    public GameObject rocky_terrain;
    public GameObject forest_terrain;
    public GameObject mountain_terrain;
    public GameObject rocky_mountain_terrain;

    public GameObject fence1;
    public GameObject fence2;

    public GameObject tree;
    public GameObject bush;
    private Level level;

    internal static int chunkLength = 6;
    
    private int offset = 0;
    internal static float bottomY = 1.28f;
    internal static float rightBound = -0.1f;
    internal static float leftBound = 1.95f;
    internal static float terrainLeftBound = 3.8f;
    internal static float terrainRightBound = -1.76f;
    internal static float biomeSeed=0;
    internal static float decorationSeed=0;

    internal static float center = (rightBound + leftBound)/2;
    public GameObject[] terrains;
    public GameObject[] fences;
    public GameObject[] trees;
    public GameObject[] colored_trees;
    public GameObject[] pine_trees;
    public GameObject[] bushes;
    public GameObject[] dead_plants;
    public GameObject[] rocks;
    public GameObject[] stumps;
    public GameObject[] flowers;
    public GameObject[] light_plants;

    public GameObject[] obsatcles;

    private LinkedList<Chunk> chunks=new();

    internal float laneWidth = Mathf.Abs(rightBound - leftBound) / 5;
    internal float[] laneCoordinates=new float[5];
    internal float[] laneBounds = new float[6];
    private int numTerrains=0;

    // Start is called before the first frame update
    void Start()
    {
        biomeSeed = Random.Range(0.0f, 10);
        decorationSeed = Random.Range(0.0f, 10);
        laneBounds[0] = leftBound;
        for (int i = 0; i < 5; i++)
        {
            laneCoordinates[i] = leftBound - laneWidth * i - laneWidth/2;
            laneBounds[i] = leftBound - laneWidth * (i+1);
            for(int j=0; j < 5; j++)
            {

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(0.5f*j, bottomY, laneBounds[i]);
                cube.transform.localScale = new Vector3(0.02f, 0.2f, 0.02f);
            }


        }
       // terrains =new GameObject[5]{plain_terrain ,rocky_terrain, forest_terrain ,mountain_terrain,rocky_mountain_terrain};
       // fences = new GameObject[2] { fence1, fence2 };
        StartCoroutine(generate());

        
        return;
       
    }
    IEnumerator generate()
    {
        int count = 20;
        for(int i=0;i<count; i++) {
            createChunk();
            yield return new WaitForSeconds(0.2f);

        }
        yield return null;
        for (int i = 0; i < 5; i++)
        {
          //  deleteEarliestChunk();
            yield return new WaitForSeconds(0.2f);

        }
        yield return null;
    }
    public static GameObject InstantiateObj(GameObject gameObject,Vector3 pos,Quaternion rotation)
    {
        return Instantiate(gameObject, pos, rotation);
    }
    public static void DestroyObj(GameObject gameObject)
    {

        Destroy(gameObject);
    }
    void generateBiome(int type,int size)
    {
        for (int i = 0; i < size; i++)
        {
           // generateChunk(type);
        }
    }

    void createChunk()
    {
        if(chunks.Count > 0) {
            chunks.Last.Value.populate();
        }
        Chunk chunk = new(numTerrains,this);
        chunk.generate();
        numTerrains++;
        chunks.AddLast(chunk);
    }
    void deleteEarliestChunk()
    {
        chunks.First.Value.OnDestroy();
        chunks.RemoveFirst();
    }
   
    void generateChunk(int type)
    {


        float startPos = numTerrains * chunkLength;
        GameObject terrain = Instantiate(terrains[type], new Vector3(0, 0, 0), Quaternion.identity);
        
        terrain.AddComponent<BoxCollider>();
        terrain.GetComponent<BoxCollider>().size = new Vector3(1.0f, 1.0f, 1.0f);
        terrain.transform.position = new Vector3(startPos, Random.Range(0.001f, 0.0f), 0);

        float fencePos = startPos-3;
        while (fencePos < startPos+chunkLength/2)
        {
            fencePos += Random.Range(0.6f, 1.5f);
            GameObject fence = Instantiate(fences[Random.Range(0, 2)], new Vector3(0, 0, 0), Quaternion.identity);
            fence.transform.position = new Vector3(fencePos, bottomY, rightBound);
            fence.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        fencePos = startPos-3;
        while (fencePos < startPos + chunkLength/2)
        {
            fencePos += Random.Range(0.6f, 1.5f);
            GameObject fence = Instantiate(fences[Random.Range(0, 2)], new Vector3(0, 0, 0), Quaternion.identity);
            fence.transform.position = new Vector3(fencePos, bottomY, leftBound);
            fence.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        numTerrains++;

    }

    // Update is called once per frame
    void Update()
    {
        //                    GameObject house = Instantiate(house_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //house.AddComponent<BoxCollider>();
    }
}
